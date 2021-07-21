using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float spinSpeed = 5f;
    public float animationSwapSpeed = 3f;
    public Transform groundCheck = null;
    
    private MasterInput playerInput = null;
    private Rigidbody playerRigidbody = new Rigidbody();
    
    private Animator animator = null;
    private Vector2 playerMoveInput = Vector2.zero;
    private Vector2 playerSpinInput = Vector2.zero;
    private Vector2 currentAnimationVector = Vector2.zero;
    
    private readonly int xPos = Animator.StringToHash("XPos");
    private readonly int yPos = Animator.StringToHash("YPos");
    private readonly int Jumping = Animator.StringToHash("Jumping");

    private HackableObject currentInteractable = null;
    
    private bool isJumping = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new MasterInput();
        playerInput.Enable();
        playerInput.Player.Movement.performed += MovePlayer;
        playerInput.Player.Movement.canceled += MoveOver;
        playerInput.Player.Jump.performed += PlayerJump;
        playerInput.Player.Camera.performed += PlayerSpin;
        playerInput.Player.Camera.canceled += PlayerSpinOver;
        playerInput.Player.Interaction.performed += Interact;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        currentAnimationVector = new Vector2(
            Mathf.Lerp(currentAnimationVector.x, playerMoveInput.x, animationSwapSpeed * Time.deltaTime),
            Mathf.Lerp(currentAnimationVector.y, playerMoveInput.y, animationSwapSpeed * Time.deltaTime));
        
        animator.SetFloat(xPos, currentAnimationVector.x);
        animator.SetFloat(yPos, currentAnimationVector.y);

        if (playerMoveInput != Vector2.zero)
        {
            transform.Translate(new Vector3(
                playerMoveInput.x * moveSpeed * Time.deltaTime,
                0,
                playerMoveInput.y * moveSpeed * Time.deltaTime));
        }

        if (playerSpinInput != Vector2.zero)
        {
            transform.Rotate(new Vector3(0, playerSpinInput.x * spinSpeed * Time.deltaTime, 0));
        }

        if (isJumping)
        {
            //for now if we are jumping it will set the jumping to false if it hits anything
            Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.2f, ~(1 << 9));
            if (colliders.Length > 0)
            {
                isJumping = false;
                animator.SetBool(Jumping, false);   
            }
        }
    }

    void Interact(InputAction.CallbackContext a_context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.BeingHacked();
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
        playerRigidbody.AddForce(Vector3.up * jumpForce);
        animator.SetBool(Jumping, true);
        isJumping = true;
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
