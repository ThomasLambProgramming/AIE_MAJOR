using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

using Malicious.Core;

namespace Malicious.Player
{
    //PlayerVariables.m_playerVariables
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        void Start()
        {
            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;
        }

        private void PlayerTick()
        {
            PlayerVariables.m_playerVariables.m_CurrentAnimationVector = new Vector2(
                Mathf.Lerp(PlayerVariables.m_playerVariables.m_CurrentAnimationVector.x, 
                    PlayerVariables.m_playerVariables.m_PlayerMoveInput.x, 
                    PlayerVariables.m_playerVariables.m_AnimationSwapSpeed * Time.deltaTime),
                Mathf.Lerp(PlayerVariables.m_playerVariables.m_CurrentAnimationVector.y, 
                    PlayerVariables.m_playerVariables.m_PlayerMoveInput.y, 
                    PlayerVariables.m_playerVariables.m_AnimationSwapSpeed * Time.deltaTime));

            PlayerVariables.m_playerVariables.m_PlayerAnimator.SetFloat(xPos, currentAnimationVector.x);
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
                

                //we dont want anything to run while in the wire except for the input to leave or if the
                //player gets to the end
                return;
            }
        }

       
        void Interact(InputAction.CallbackContext a_context)
        {
            if (currentInteractable != null)
            {
                //add dot product checks and etc
                currentInteractable.Hacked();
            }
            else if (currentPlayer != truePlayerObject)
            {
                //Run set to true player
                ResetToTruePlayer(currentPlayer.transform.position);
            }
        }
        void ResetToTruePlayer(Vector3 a_position)
        {
            
            truePlayerObject.SetActive(true);
            a_position.y += 1f;
            truePlayerObject.transform.position = a_position;
        }

        void SetToCurrentMoveable()
        {
            
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
            PlayerVariables.m_playerVariables.m_PlayerInput.Disable();
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Movement.performed -= MovePlayer;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Movement.canceled -= MoveOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Jump.performed -= PlayerJump;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Jump.canceled -= PlayerJumpOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Camera.performed -= PlayerSpin;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Camera.canceled -= PlayerSpinOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Interaction.performed -= Interact;
        }

        private void EnableInput()
        {
            PlayerVariables.m_playerVariables.m_PlayerInput.Enable();
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Movement.performed += MovePlayer;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Movement.canceled += MoveOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Jump.performed += PlayerJump;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Jump.canceled += PlayerJumpOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Camera.performed += PlayerSpin;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Camera.canceled += PlayerSpinOver;
            PlayerVariables.m_playerVariables.m_PlayerInput.Player.Interaction.performed += Interact;
        }
    }
}