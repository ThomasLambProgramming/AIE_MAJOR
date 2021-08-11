using System.Collections.Generic;
using Cinemachine;
using Malicious.Hackable;
using Malicious.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Core
{
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        //We can assume there will be only one player
        public static PlayerController MainPlayer;
        
        [SerializeField] private CinemachineVirtualCamera mainCam = null;
        [SerializeField] private GameObject truePlayerObject = null;

        private GameObject currentPlayer = null;
        private Rigidbody currentPlayerRigidbody = null;

        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public float spinSpeed = 5f;
        public float animationSwapSpeed = 3f;
        public Transform groundCheck = null;

        private bool rotateWireCam = false;
        private Quaternion rotationGoalWire = Quaternion.identity;
        [SerializeField] private float rotationSpeed = 10f;

        private bool canJump = true;
    
        private MasterInput playerInput = null;

        private Animator animator = null;
        private Vector2 playerMoveInput = Vector2.zero;
        private Vector2 playerSpinInput = Vector2.zero;
        private Vector2 currentAnimationVector = Vector2.zero;

        private readonly int xPos = Animator.StringToHash("XPos");
        private readonly int yPos = Animator.StringToHash("YPos");
        private readonly int jumping = Animator.StringToHash("Jumping");

        private IHackableMovement currentMoveable = null;
        private IHackableInteractable currentInteractable = null;

        [SerializeField] private float wireSpeed = 20f;
        [SerializeField] private GameObject wireDummy = null;
        [SerializeField] private GameObject wireCameraOffset = null;
        private List<Vector3> wirePath = null;
        private float goNextDistance = 0.2f;
        private bool inWire = false;
        private int pathIndex = 0;
        private bool isJumping = false;

        private float jumpTimer = 0.3f;
        private float jumpStopTimer = 0;

        private bool hasDoubleJumped = false;

        private bool holdingJump = false;

        // Start is called before the first frame update
        void Awake()
        {
            playerInput = new MasterInput();
            MainPlayer = this;
        }

        private Vector3 cameraOffset = Vector3.zero;

        private void Start()
        {
            currentPlayer = truePlayerObject;
            animator = currentPlayer.GetComponent<Animator>();
            currentPlayerRigidbody = truePlayerObject.GetComponent<Rigidbody>();
            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;
        }

        private void PlayerTick()
        {
            currentAnimationVector = new Vector2(
                Mathf.Lerp(currentAnimationVector.x, playerMoveInput.x, animationSwapSpeed * Time.deltaTime),
                Mathf.Lerp(currentAnimationVector.y, playerMoveInput.y, animationSwapSpeed * Time.deltaTime));

            animator.SetFloat(xPos, currentAnimationVector.x);
            animator.SetFloat(yPos, currentAnimationVector.y);
        }
        //Had to name this function something else then fixedupdate so it can be on events not the monobehaviour
        private void FixedTick()
        {
            

            //FIX THE ANIMATOR FOR WHEN EXITED HACKED OBJECT (fine for proof of concept)
            if (playerSpinInput != Vector2.zero)
            {
                currentPlayer.transform.Rotate(new Vector3(0, playerSpinInput.x * spinSpeed * Time.deltaTime, 0));
            }

            if (playerMoveInput != Vector2.zero)
            {
                float currentYAmount = currentPlayerRigidbody.velocity.y;
                Vector3 newVel =
                    currentPlayer.transform.forward * (playerMoveInput.y * moveSpeed * Time.deltaTime) +
                    currentPlayer.transform.right * (playerMoveInput.x * moveSpeed * Time.deltaTime);
                newVel.y = currentYAmount;
                currentPlayerRigidbody.velocity = newVel;
            }

            if (Mathf.Abs(playerMoveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(currentPlayerRigidbody.velocity.x) > 0.2f || Mathf.Abs(currentPlayerRigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = currentPlayerRigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * 0.95f;
                    newVel.x = newVel.x * 0.95f;
                    currentPlayerRigidbody.velocity = newVel;
                }
            }


            if (isJumping)
            {
            
                if (currentPlayerRigidbody.velocity.y < 0 && holdingJump == false)
                {
                    Vector3 grav = new Vector3(0, -9.8f, 0);
                    currentPlayerRigidbody.velocity += grav * Time.deltaTime;
                }
                else if (holdingJump == false)
                {
                    Vector3 grav = new Vector3(0, -9.8f, 0);
                    currentPlayerRigidbody.velocity += grav * Time.deltaTime;
                }

                jumpStopTimer += Time.deltaTime;
                if (jumpStopTimer >= jumpTimer)
                {
                    //for now if we are jumping it will set the jumping to false if it hits anything
                    Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.1f, ~(1 << 10));
                    if (colliders.Length > 0)
                    {
                        hasDoubleJumped = false;
                        isJumping = false;
                        canJump = true;
                        //animator.SetBool(jumping, false);   
                    }
                }
            }
        }

        private void inWireUpdate()
        {
            if (inWire)
            {
                if (Vector3.Distance(wireDummy.transform.position, wirePath[pathIndex]) > goNextDistance)
                {
                    Vector3 currentWirePos = wireDummy.transform.position;
                    currentWirePos = currentWirePos + (wirePath[pathIndex] - wireDummy.transform.position).normalized *
                        (Time.deltaTime * (wireSpeed));
                    wireDummy.transform.position = currentWirePos;
                    if (rotateWireCam)
                    {
                        wireDummy.transform.rotation = Quaternion.RotateTowards(
                            wireDummy.transform.rotation,
                            rotationGoalWire,
                            rotationSpeed);
                        if (wireDummy.transform.rotation == rotationGoalWire)
                            rotateWireCam = false;
                    }
                }
                else
                {
                    if (pathIndex < wirePath.Count - 1)
                    {
                        pathIndex++;
                        rotationGoalWire = Quaternion.LookRotation(
                            (wirePath[pathIndex] - wireDummy.transform.position).normalized, Vector3.up);
                        rotateWireCam = true;
                    }
                    else
                    {
                        //end of path
                        ResetToTruePlayer(wireDummy.transform.position);
                        pathIndex = 0;
                        wirePath = null;
                        inWire = false;
                        wireDummy.SetActive(false);
                        mainCam.Follow = currentPlayer.transform;
                        truePlayerObject.transform.position = wireDummy.transform.position;
                        GameEventManager.PlayerFixedUpdate -= inWireUpdate;
                    }
                }

                //we dont want anything to run while in the wire except for the input to leave or if the
                //player gets to the end
                return;
            }
        }

       
        void Interact(InputAction.CallbackContext a_context)
        {
            //Discuss the importance the order may have to be changed based on game feel to have importance
            //of the interactables over the moveables
            if (currentInteractable != null)
            {
                //add dot product checks and etc
                currentInteractable.Hacked();
            }
            else if (currentMoveable != null)
            {
                MovementHackables currentType = currentMoveable.TypeOfHackable();
                switch (currentType)
                {
                    case MovementHackables.MoveableObject:
                        SetToCurrentMoveable();
                        break;
                    case MovementHackables.Wire:
                        if (!inWire)
                            EnterWire();
                        break;
                    case MovementHackables.GroundAgent:
                        break;
                    case MovementHackables.FlyingAgent:
                        break;
                }
            }
            else if (currentPlayer != truePlayerObject)
            {
                //set it to be kinematic for now so no weird bugs can occur
                currentPlayerRigidbody.isKinematic = true;
                Vector3 spawnPoint = currentPlayer.transform.position;
                truePlayerObject.SetActive(true);
                spawnPoint.y += currentPlayer.transform.localScale.y + 0.3f;
                truePlayerObject.transform.position = spawnPoint;
                truePlayerObject.transform.rotation = currentPlayer.transform.rotation;
                currentPlayer = truePlayerObject;
                mainCam.Follow = currentPlayer.transform;
                currentPlayerRigidbody = currentPlayer.GetComponent<Rigidbody>();
                canJump = true;
            }
        }
        /// <summary>
        /// This is to only be used when a moveable hackable has been hacked
        /// not for interactables
        /// </summary>
        void ResetToTruePlayer(Vector3 a_position)
        {
            
            truePlayerObject.SetActive(true);
            a_position.y += 1f;
            truePlayerObject.transform.position = a_position;
        }

        void SetToCurrentMoveable()
        {
            MovementObjectInformation objectInfo = currentMoveable.GiveObjectInformation();
            currentPlayer = objectInfo.hackableObject;
            currentPlayerRigidbody = objectInfo.hackableRigigbody;
            if (currentPlayerRigidbody != null && currentPlayerRigidbody.isKinematic == true)
                currentPlayerRigidbody.isKinematic = false;
            currentMoveable = null;
            currentInteractable = null;
            truePlayerObject.SetActive(false);
            canJump = false;
            hasDoubleJumped = true;
            mainCam.Follow = currentPlayer.transform;
            /*
             * Figure out what we want to do for the rotation of the object (this might need to be tweaked alot)
             */
        }

        void EnterWire()
        {
            GameObject wireObject = currentMoveable.GiveObjectInformation().hackableObject;
            wirePath = wireObject.GetComponent<Wire>().GivePath();
            mainCam.Follow = wireCameraOffset.transform;
            truePlayerObject.SetActive(false);
            wireDummy.transform.position = wirePath[0];
            wireDummy.SetActive(true);
            inWire = true;
            currentInteractable = null;
            pathIndex = 1;
            wireDummy.transform.rotation = Quaternion.LookRotation((wirePath[pathIndex] - wireDummy.transform.position).normalized, Vector3.up);
            GameEventManager.PlayerFixedUpdate += inWireUpdate;
        }
        void MovePlayer(InputAction.CallbackContext a_context)
        {
            playerMoveInput = a_context.ReadValue<Vector2>();
        }
        void MoveOver(InputAction.CallbackContext a_context)
        {
            playerMoveInput = Vector2.zero;
        }
        void PlayerJump(InputAction.CallbackContext a_context)
        {
            holdingJump = true;
            if (inWire)
            {
                //end of path
                pathIndex = 0;
                wirePath = null;
                inWire = false;
                wireDummy.SetActive(false);
                mainCam.Follow = currentPlayer.transform;
                truePlayerObject.SetActive(true);
                Vector3 newPlayerPos = wireDummy.transform.position;
                newPlayerPos.y += 1.5f;
                truePlayerObject.transform.position = newPlayerPos;
                GameEventManager.PlayerFixedUpdate -= inWireUpdate;
                return;
            }

            if (canJump || hasDoubleJumped == false)
            {
                currentPlayerRigidbody.velocity = (Vector3.up * jumpForce);
                //animator.SetBool(jumping, true);
                isJumping = true;
                jumpStopTimer = 0;
                if (hasDoubleJumped == false && canJump == false)
                {
                    hasDoubleJumped = true;
                }

                canJump = false;
            }
        }
        void PlayerJumpOver(InputAction.CallbackContext a_context)
        {
            holdingJump = false;
        }
        void PlayerSpin(InputAction.CallbackContext a_context)
        {
            playerSpinInput = a_context.ReadValue<Vector2>();
        }
        void PlayerSpinOver(InputAction.CallbackContext a_context)
        {
            playerSpinInput = Vector2.zero;
        }

        public void SetMoveable(IHackableMovement a_moveable) => currentMoveable = a_moveable;
        public void SetInteractable(IHackableInteractable a_interactable) => currentInteractable = a_interactable;
        public void RemoveInteractable() => currentInteractable = null;

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
            playerInput.Disable();
            playerInput.Player.Movement.performed -= MovePlayer;
            playerInput.Player.Movement.canceled -= MoveOver;
            playerInput.Player.Jump.performed -= PlayerJump;
            playerInput.Player.Jump.canceled -= PlayerJumpOver;
            playerInput.Player.Camera.performed -= PlayerSpin;
            playerInput.Player.Camera.canceled -= PlayerSpinOver;
            playerInput.Player.Interaction.performed -= Interact;
        }

        private void EnableInput()
        {
            playerInput.Enable();
            playerInput.Player.Movement.performed += MovePlayer;
            playerInput.Player.Movement.canceled += MoveOver;
            playerInput.Player.Jump.performed += PlayerJump;
            playerInput.Player.Jump.canceled += PlayerJumpOver;
            playerInput.Player.Camera.performed += PlayerSpin;
            playerInput.Player.Camera.canceled += PlayerSpinOver;
            playerInput.Player.Interaction.performed += Interact;
        }
    }
}