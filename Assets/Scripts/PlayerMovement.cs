using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCam = null;

    //the game object used will change (when the player moves to be an object for a short period)
    private GameObject truePlayerObject = null;
    [SerializeField] private GameObject playerCamFollow = null;

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

    private HackableObject currentInteractable = null;

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
        truePlayerObject = gameObject;
        currentPlayer = truePlayerObject;
        playerInput = new MasterInput();
    }

    private Vector3 cameraOffset = Vector3.zero;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentPlayerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        currentAnimationVector = new Vector2(
            Mathf.Lerp(currentAnimationVector.x, playerMoveInput.x, animationSwapSpeed * Time.deltaTime),
            Mathf.Lerp(currentAnimationVector.y, playerMoveInput.y, animationSwapSpeed * Time.deltaTime));

        animator.SetFloat(xPos, currentAnimationVector.x);
        animator.SetFloat(yPos, currentAnimationVector.y);
    }

    private void FixedUpdate()
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
                    pathIndex = 0;
                    wirePath = null;
                    inWire = false;
                    wireDummy.SetActive(false);
                    mainCam.Follow = playerCamFollow.transform;
                    truePlayerObject.transform.position = wireDummy.transform.position;
                }
            }

            //we dont want anything to run while in the wire except for the input to leave or if the
            //player gets to the end
            return;
        }

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


        if (isJumping)
        {
            Debug.Log(holdingJump);
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


    void Interact(InputAction.CallbackContext a_context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.BeingHacked(out HackedType objectType, out Transform a_cameraFollow);
            if (objectType == HackedType.Enemy || objectType == HackedType.MoveableObject)
            {
                currentPlayer = currentInteractable.gameObject;
                currentPlayerRigidbody = currentPlayer.GetComponent<Rigidbody>();
                if (currentPlayerRigidbody.isKinematic)
                    currentPlayerRigidbody.isKinematic = false;
                currentInteractable = null;
                currentPlayer.transform.rotation = truePlayerObject.transform.rotation;
                truePlayerObject.transform.position = new Vector3(0, -900, 0);
                mainCam.Follow = a_cameraFollow;
                canJump = false;
            }
            else if (objectType == HackedType.Wire)
            {
                wirePath = currentInteractable.GetComponent<Wire>().GivePath();
                mainCam.Follow = wireCameraOffset.transform;
                truePlayerObject.transform.position = new Vector3(0, -900, 0);
                wireDummy.transform.position = wirePath[0];
                wireDummy.SetActive(true);
                inWire = true;
                currentInteractable = null;
                //we always want to move to the next index not the starting position (because we are already there)
                //and it might cause issues
                pathIndex = 1;
                wireDummy.transform.rotation =
                    Quaternion.LookRotation((wirePath[pathIndex] - wireDummy.transform.position).normalized,
                        Vector3.up);
            }
            //run functions for the object type or etc
        }
        else if (currentPlayer != truePlayerObject)
        {
            //set it to be kinematic for now so no weird bugs can occur
            currentPlayerRigidbody.isKinematic = true;
            Vector3 spawnPoint = currentPlayer.transform.position;

            spawnPoint.y += currentPlayer.transform.localScale.y + 0.3f;
            truePlayerObject.transform.position = spawnPoint;
            truePlayerObject.transform.rotation = currentPlayer.transform.rotation;
            currentPlayer = truePlayerObject;
            mainCam.Follow = playerCamFollow.transform;
            currentPlayerRigidbody = currentPlayer.GetComponent<Rigidbody>();
            canJump = true;
        }
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
            mainCam.Follow = playerCamFollow.transform;

            Vector3 newPlayerPos = wireDummy.transform.position;
            newPlayerPos.y += 1.5f;
            truePlayerObject.transform.position = newPlayerPos;

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

    //Functions for setting the interactable object for the player (its assumed that there will be only one
    //object the player will "hack at a time")
    public void SetInteractable(HackableObject a_hackableObject)
    {
        currentInteractable = a_hackableObject;
    }

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