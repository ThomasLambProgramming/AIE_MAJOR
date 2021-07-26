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
    
    private bool isJumping = false;
    
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
        
    }

    private void FixedUpdate()
    {
        //FIX THE ANIMATOR FOR WHEN EXITED HACKED OBJECT (fine for proof of concept)
        currentAnimationVector = new Vector2(
            Mathf.Lerp(currentAnimationVector.x, playerMoveInput.x, animationSwapSpeed * Time.deltaTime),
            Mathf.Lerp(currentAnimationVector.y, playerMoveInput.y, animationSwapSpeed * Time.deltaTime));
        
        animator.SetFloat(xPos, currentAnimationVector.x);
        animator.SetFloat(yPos, currentAnimationVector.y);
        

        if (playerSpinInput != Vector2.zero)
        {
            currentPlayer.transform.Rotate(new Vector3(0, playerSpinInput.x * spinSpeed * Time.deltaTime, 0));
        }
        if (playerMoveInput != Vector2.zero)
        {
            float currentYAmount = currentPlayerRigidbody.velocity.y;
            Vector3 newVel = 
                currentPlayer.transform.forward * (playerMoveInput.y * moveSpeed * Time.deltaTime) +
                currentPlayer.transform.right *  (playerMoveInput.x * moveSpeed * Time.deltaTime);
            newVel.y = currentYAmount;
            currentPlayerRigidbody.velocity = newVel;
        }
        if (isJumping)
        {
            //for now if we are jumping it will set the jumping to false if it hits anything
            Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.1f, ~(1 << 10));
            if (colliders.Length > 0)
            {
                Debug.Log("found");
                isJumping = false;
                canJump = true;
                //animator.SetBool(jumping, false);   
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
            //run functions for the object type or etc
        }
        else if (currentPlayer != truePlayerObject)
        {
            //set it to be kinematic for now so no weird bugs can occur
            currentPlayerRigidbody.isKinematic = true;
            truePlayerObject.transform.position = currentPlayer.transform.position;
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
        if (canJump)
        {
            currentPlayerRigidbody.AddForce(Vector3.up * jumpForce);
            //animator.SetBool(jumping, true);
            isJumping = true;
            canJump = false;
        }
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
        playerInput.Enable();
        playerInput.Player.Movement.performed += MovePlayer;
        playerInput.Player.Movement.canceled += MoveOver;
        playerInput.Player.Jump.performed += PlayerJump;
        playerInput.Player.Camera.performed += PlayerSpin;
        playerInput.Player.Camera.canceled += PlayerSpinOver;
        playerInput.Player.Interaction.performed += Interact;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Movement.performed -= MovePlayer;
        playerInput.Player.Movement.canceled -= MoveOver;
        playerInput.Player.Jump.performed -= PlayerJump;
        playerInput.Player.Camera.performed -= PlayerSpin;
        playerInput.Player.Camera.canceled -= PlayerSpinOver;
        playerInput.Player.Interaction.performed -= Interact;
    }
}
