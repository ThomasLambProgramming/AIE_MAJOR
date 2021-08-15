using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

using Malicious.Core;
using Malicious.Hackable;
using Malicious.Interfaces;

namespace Malicious.Player
{
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController PlayerControl = null;
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
        public IHackable m_currentInteractable = null;
        [SerializeField] private ObjectType m_currentPlayerType = ObjectType.TruePlayer;
        
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
        [HideInInspector] public bool m_inWire = false;
        

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
            PlayerControl = this;
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
            switch (m_currentPlayerType)
            {
                case ObjectType.TruePlayer:
                    m_playerMovement.SpinMove(m_playerSpinInput, m_spinSpeed);
                    break;
                case ObjectType.MoveableObject:
                    m_playerMovement.SpinMove(m_playerSpinInput, m_spinSpeed);
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
        //Had to name this function something else then fixedupdate so it can be on events not the monobehaviour
        private void FixedTick()
        {
            //Current logic running for the active start
            switch (m_currentPlayerType)
            {
                case ObjectType.TruePlayer:
                    m_playerMovement.StandardMove(m_playerMoveInput, m_moveSpeed);
                    break;
                case ObjectType.MoveableObject:
                    m_playerMovement.HackObjectMove(m_playerMoveInput, m_moveSpeed, m_cameraOffset);
                    break;
                case ObjectType.Wire:
                    m_playerMovement.WireMove();
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
            if (m_currentPlayerObject != m_truePlayerObject)
            {
                //Run set to true player
                SwapPlayer(0);
            }
            else if (m_currentInteractable != null)
            {
                //add dot product checks and etc
                m_currentInteractable.Hacked();
                SwapPlayer(m_currentInteractable.GiveInformation().m_objectType);
            }
            
        }
        /// <summary>
        /// Swaps the current player to any of the hackable types or the original player
        /// this is used to have clean transitions without overwriting code
        /// </summary>
        public void SwapPlayer(ObjectType a_type)
        {
            //Unloading current type
            switch (m_currentPlayerType)
            {
                case ObjectType.TruePlayer:
                    
                    break;
                case ObjectType.MoveableObject:
                    m_currentInteractable.PlayerExit();
                    m_truePlayerObject.transform.rotation = new Quaternion(0, m_cameraOffset.rotation.y, 0, m_cameraOffset.rotation.w);
                    break;
                case ObjectType.Wire:
                    m_wireModel.SetActive(false);
                    m_playerMovement.m_pathIndex = 0;
                    m_playerMovement.m_wirePath = null;
                    m_playerMovement.m_rotationGoal = Quaternion.identity;
                    m_truePlayerObject.transform.position = m_wireModel.transform.position;
                    m_truePlayerObject.transform.rotation = 
                        new Quaternion(
                            0, 
                            m_wireModel.transform.rotation.y, 
                            0, 
                            m_wireModel.transform.rotation.z);
                    m_currentInteractable.PlayerExit();
                    break;
                case ObjectType.GroundEnemy:
                    m_currentInteractable.PlayerExit();
                    break;
                case ObjectType.FlyingEnemy:
                    m_currentInteractable.PlayerExit();                    
                    break;
            }

            bool updateMovement = true;
            bool updateRigid = true;
            //loading current type
            switch (a_type)
            {
                case ObjectType.TruePlayer:
                    updateRigid = false;
                    SetToPlayer();
                    break;
                case ObjectType.MoveableObject:
                    //since we are not moving the camera from its original rotation the offset on the moveable needs to be the
                    //same as the current object
                    m_currentInteractable.GiveInformation().m_cameraOffset.rotation = m_cameraOffset.transform.rotation;
                    SetToMoveable();
                    break;
                case ObjectType.Wire:
                    updateRigid = false;
                    SetToWire();
                    break;
                case ObjectType.GroundEnemy:
                    SetToGroundEnemy();
                    break;
                case ObjectType.FlyingEnemy:
                    SetToFlyEnemy();
                    break;
                case ObjectType.ControlPanel:
                    m_currentInteractable.Hacked();
                    updateMovement = false;
                    updateRigid = false;
                    break;
            }


            if (updateRigid)
            {
                HackableInformation objectInformation = m_currentInteractable.GiveInformation();
                m_currentRigidbody = objectInformation.m_rigidBody;
                m_currentPlayerObject = objectInformation.m_object;
                m_cameraOffset = objectInformation.m_cameraOffset;
                
                if (objectInformation.m_cameraOffset != null)
                    m_mainCam.Follow = m_cameraOffset;
                
                if (m_currentRigidbody.isKinematic == true)
                    m_currentRigidbody.isKinematic = false;
            }
            if (updateMovement)
            {
                m_currentPlayerType = a_type;
                m_playerMovement.UpdatePlayer(m_currentPlayerObject, m_currentRigidbody);
                m_mainCam.Follow = m_cameraOffset;
            }
            
        }

        private void SetToPlayer()
        {
            m_truePlayerObject.SetActive(true);
            
            Vector3 newpos = m_currentPlayerObject.transform.position;
            newpos.y += 1;
            m_truePlayerObject.transform.position = newpos;
            
            m_currentPlayerObject = m_truePlayerObject;
            m_currentRigidbody = m_trueRigidbody;
            m_cameraOffset = m_trueCameraOffset;
            m_currentInteractable = null;
        }

        private void SetToMoveable()
        {
            m_truePlayerObject.SetActive(false);
        }

        private void SetToWire()
        {
            m_truePlayerObject.SetActive(false);
            m_wireModel.SetActive(true);
            HackableInformation wireInfo = m_currentInteractable.GiveInformation();
            Malicious.Hackable.Wire wireScript = wireInfo.m_object.GetComponent<Wire>();
            m_playerMovement.m_wirePath = wireScript.GivePath();
            m_currentPlayerObject = m_wireModel;
            m_currentRigidbody = null;
            m_cameraOffset = m_wireCameraOffset;
            m_wireModel.transform.position = m_playerMovement.m_wirePath[0];
            //At the moment this assumes we have a path
            //we get the direction from the start to the next node and face the wire object in that direction to 
            //start so we arent looking in the wrong direction
            Vector3 directionToNode = (m_playerMovement.m_wirePath[1] - m_playerMovement.m_wirePath[0]).normalized;
            Quaternion newRotation = Quaternion.LookRotation(directionToNode);
            m_currentPlayerObject.transform.rotation = newRotation;
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