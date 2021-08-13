using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


using Malicious.Core;
using Malicious.Hackable;
using Malicious.Interfaces;

namespace Malicious.Player
{
    /// <summary>
    /// This is for player swapping to be on a easy to read switch function
    /// </summary>
    public enum ObjectType
    {
        TruePlayer = 0,
        MoveableObject = 1,
        Wire = 2,
        GroundEnemy = 3,
        FlyingEnemy = 4
    }
    
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        private PlayerMovement m_playerMovement = null;
        
        //------Camera Variables-------------------//
        [Space(10f)]
        [Header("Camera Variables")]
        public CinemachineVirtualCamera m_mainCam = null;
            //Universal camera offset              
        public Transform m_cameraOffset = null;
            //This is the default for the player that the offset will always be based off
        [HideInInspector] public Transform m_trueCameraOffset = null;
        
        
        //------Player Object Variables------------//
        [Space(10f)]
        [Header("Player Object Variables")]
        public GameObject m_truePlayerObject = null;
        [HideInInspector] public Rigidbody m_trueRigidbody = null;
        [HideInInspector] public GameObject m_currentPlayerObject = null;
        [HideInInspector] public Rigidbody m_currentRigidbody = null;


        //------Hackable Variables-----------------//
        [HideInInspector] public IHackable m_currentInteractable = null;
        [HideInInspector] private ObjectType m_currentPlayerType = ObjectType.TruePlayer;
        
        //------True Player Variables--------------//
        [Space(10f)]
        [Header("Player Variables")]
        public float m_moveSpeed = 5f;
        public float m_jumpForce = 10f;
        public float m_spinSpeed = 5f;


        //------Jumping Variables--------------//
        [Space(10f)]
        [Header("Jumping Variables")]
        [HideInInspector] public bool m_canJump = true;
        [HideInInspector] public bool m_hasDoubleJumped = false;
        [HideInInspector] public bool m_holdingJump = false;
        public Transform m_groundCheck = null;


        //------Wire Variables-----------------//
        [Space(10f)]
        [Header("Wire Variables")]
        public GameObject m_wireModel = null;
        public Transform m_wireCameraOffset = null;
        public float m_wireSpeed = 10f;
        [HideInInspector] public bool m_inWire = false;
        [HideInInspector] public int m_pathIndex = 0;
        [HideInInspector] public float m_goNextWire = 0.2f;
        [HideInInspector] public List<Vector3> m_wirePath = null;
        [HideInInspector] public Quaternion m_rotationGoal = Quaternion.identity;
        [HideInInspector] public float m_rotateSpeed = 10f;
        
        
        //------Player Input variables---------//
        public MasterInput m_playerInput = null;
        [HideInInspector] public Vector2 m_playerMoveInput = Vector2.zero;
        [HideInInspector] public Vector2 m_playerSpinInput = Vector2.zero;

        
        //------Animator Variables-------------//
        [Space(10f)]
        [Header("Animator Variables")]
        public float m_animationSwapSpeed = 3f;
        [HideInInspector] public Animator m_playerAnimator = null;
        [HideInInspector] public Vector2 m_currentAnimationVector = Vector2.zero;
        public readonly int m_xPos = Animator.StringToHash("XPos");
        public readonly int m_yPos = Animator.StringToHash("YPos");
        public readonly int m_jumping = Animator.StringToHash("Jumping");
        
        #endregion
        void Awake()
        {
            m_playerInput = new MasterInput();
        }

        void Start()
        {
            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;
            
            m_currentPlayerObject = m_truePlayerObject;
            m_trueRigidbody = m_truePlayerObject.GetComponent<Rigidbody>();
            m_currentRigidbody = m_trueRigidbody;
            m_playerAnimator = m_currentPlayerObject.GetComponent<Animator>();
            m_playerMovement = new PlayerMovement(m_currentPlayerObject, m_currentRigidbody);
            m_mainCam.Follow = m_cameraOffset;
            m_trueCameraOffset = m_cameraOffset;
        }
        private void PlayerTick()
        {
            UpdateAnimator();
        }
        //Had to name this function something else then fixedupdate so it can be on events not the monobehaviour
        private void FixedTick()
        {
            //Current logic running for the active start
            switch (m_currentPlayerType)
            {
                case ObjectType.TruePlayer:
                    m_playerMovement.StandardMove(m_playerMoveInput, m_moveSpeed);
                    m_playerMovement.SpinMove(m_playerSpinInput, m_spinSpeed);
                    break;
                case ObjectType.MoveableObject:
                    break;
                case ObjectType.Wire:
                    break;
                case ObjectType.GroundEnemy:
                    break;
                case ObjectType.FlyingEnemy:
                    break;
                default:
                    Debug.LogWarning("ERROR PLAYER HAS NO TYPE");
                    break;
            }
            
        }
        void Interact(InputAction.CallbackContext a_context)
        {
            if (m_currentInteractable != null)
            {
                //add dot product checks and etc
                m_currentInteractable.Hacked();
            }
            else if (m_currentPlayerObject != m_truePlayerObject)
            {
                //Run set to true player
                SwapPlayer(0);
            }
        }
        /// <summary>
        /// Swaps the current player to any of the hackable types or the original player
        /// this is used to have clean transitions without overwriting code
        /// </summary>
        private void SwapPlayer(ObjectType a_type)
        {
            switch (a_type)
            {
                case ObjectType.TruePlayer:
                    SetToPlayer();
                    break;
                case ObjectType.MoveableObject:
                    SetToMoveable();
                    break;
                case ObjectType.Wire:
                    SetToWire();
                    break;
                case ObjectType.GroundEnemy:
                    SetToGroundEnemy();
                    break;
                case ObjectType.FlyingEnemy:
                    SetToFlyEnemy();
                    break;
            }
        }

        private void SetToPlayer()
        {
            m_currentInteractable.PlayerExit();
            m_currentPlayerObject = m_truePlayerObject;
            m_currentRigidbody = m_trueRigidbody;
            m_cameraOffset = m_trueCameraOffset;
        }

        private void SetToMoveable()
        {
            m_truePlayerObject.SetActive(false);
            HackableInformation objectInformation = m_currentInteractable.GiveInformation();
            m_currentRigidbody = objectInformation.m_rigidBody;
            m_currentPlayerObject = objectInformation.m_object;
            m_cameraOffset = objectInformation.m_cameraOffset;
            if (m_currentRigidbody.isKinematic == true)
                m_currentRigidbody.isKinematic = false;
        }

        private void SetToWire()
        {
            m_truePlayerObject.SetActive(false);
            HackableInformation wireInfo = m_currentInteractable.GiveInformation();
            Malicious.Hackable.Wire wireScript = wireInfo.m_object.GetComponent<Wire>();
            m_wirePath = wireScript.GivePath();
            m_currentPlayerObject = m_wireModel;
            m_currentRigidbody = null;
            m_cameraOffset = m_wireCameraOffset;
            
            //At the moment this assumes we have a path
            Vector3 directionToNode = 
        }

        private void SetToGroundEnemy()
        {
            
        }

        private void SetToFlyEnemy()
        {
            
        }

        
        private void MoveInputPlayer(InputAction.CallbackContext a_context)
        {
            m_playerMoveInput = a_context.ReadValue<Vector2>();
        }
        private void MoveInputOver(InputAction.CallbackContext a_context)
        {
            m_playerMoveInput = Vector2.zero;
        }
        private void PlayerJumpInput(InputAction.CallbackContext a_context)
        {
            //Conditional for if in ai no jump
            //Normal Jump Function
            //In Wire Jump
        }
        private void PlayerJumpInputOver(InputAction.CallbackContext a_context)
        {
           m_holdingJump = false;
        }
        private void PlayerSpinInput(InputAction.CallbackContext a_context)
        {
           m_playerSpinInput = a_context.ReadValue<Vector2>();
        }
        private void PlayerSpinInputOver(InputAction.CallbackContext a_context)
        {
           m_playerSpinInput = Vector2.zero;
        }
        
        public void SetInteractable(IHackable a_interactable) => m_currentInteractable = a_interactable;
        public void RemoveInteractable() => m_currentInteractable = null;
        private void UpdateAnimator()
        {
            if (m_currentPlayerObject != m_truePlayerObject)
                return;
            m_currentAnimationVector = new Vector2(
                Mathf.Lerp(m_currentAnimationVector.x, 
                    m_playerMoveInput.x, 
                    m_animationSwapSpeed * Time.deltaTime),
                Mathf.Lerp(m_currentAnimationVector.y, 
                    m_playerMoveInput.y, 
                    m_animationSwapSpeed * Time.deltaTime));

            m_playerAnimator.SetFloat(m_xPos, m_currentAnimationVector.x);
            m_playerAnimator.SetFloat(m_yPos, m_currentAnimationVector.y);
        }

        private void OnEnable()
        {
            EnableInput();
        }
        
        private void OnDisable()
        {
            DisableInput();
        }
        
        private void DisableInput()
        {
            m_playerInput.Disable();
            m_playerInput.Player.Movement.performed -= MoveInputPlayer;
            m_playerInput.Player.Movement.canceled -= MoveInputOver;
            m_playerInput.Player.Jump.performed -= PlayerJumpInput;
            m_playerInput.Player.Jump.canceled -= PlayerJumpInputOver;
            m_playerInput.Player.Camera.performed -= PlayerSpinInput;
            m_playerInput.Player.Camera.canceled -= PlayerSpinInputOver;
            m_playerInput.Player.Interaction.performed -= Interact;
        }
        
        private void EnableInput()
        {
            m_playerInput.Enable();
            m_playerInput.Player.Movement.performed += MoveInputPlayer;
            m_playerInput.Player.Movement.canceled += MoveInputOver;
            m_playerInput.Player.Jump.performed += PlayerJumpInput;
            m_playerInput.Player.Jump.canceled += PlayerJumpInputOver;
            m_playerInput.Player.Camera.performed += PlayerSpinInput;
            m_playerInput.Player.Camera.canceled += PlayerSpinInputOver;
            m_playerInput.Player.Interaction.performed += Interact;
        }
    }
}