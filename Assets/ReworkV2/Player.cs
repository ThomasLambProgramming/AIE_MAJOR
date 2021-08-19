using System;
using Malicious.Player;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Malicious.ReworkV2
{
    public class Player : MonoBehaviour, IPlayerObject
    {
        [SerializeField] private PlayerValues _values = new PlayerValues();

        private void Start()
        {
            _values._rigidbody = GetComponent<Rigidbody>();
            _values._playerAnimator = GetComponent<Animator>();
        }

        public void OnHackEnter()
        {
            EnableInput();
        }

        public void OnHackExit()
        {
            DisableInput();
        }

        public void Tick()
        {
            //Logic
            UpdateAnimator();
            SpinMovement();
        }

        public void FixedTick()
        {
            //Movement and etc
            Movement();
        }

        private void Movement()
        {
            if (_values._moveInput != Vector2.zero)
            {
                float currentYAmount = _values._rigidbody.velocity.y;
                Vector3 newVel =
                    transform.forward * (_values._moveInput.y * _values._moveSpeed * Time.deltaTime) +
                    transform.right * (_values._moveInput.x * _values._moveSpeed * Time.deltaTime);
                newVel.y = currentYAmount;
                _values._rigidbody.velocity = newVel;
            }
            
            if (Mathf.Abs(_values._moveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(_values._rigidbody.velocity.x) > 0.2f || Mathf.Abs(_values._rigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = _values._rigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * 0.95f;
                    newVel.x = newVel.x * 0.95f;
                    _values._rigidbody.velocity = newVel;
                }
            }
        }
        private void UpdateAnimator()
        {
            _values._currentAnimationVector = new Vector2(
                Mathf.Lerp(_values._currentAnimationVector.x, 
                    _values._moveInput.x, 
                    _values._animationSwapSpeed * Time.deltaTime),
                Mathf.Lerp(_values._currentAnimationVector.y, 
                    _values._moveInput.y, 
                    _values._animationSwapSpeed * Time.deltaTime));
        
            _values._playerAnimator.SetFloat(_values._xPos, _values._currentAnimationVector.x);
            _values._playerAnimator.SetFloat(_values._yPos, _values._currentAnimationVector.y);
        }
        private void SpinMovement()
        {
            if (_values._spinInput != Vector2.zero)
            {
                transform.Rotate(new Vector3(0, _values._spinInput.x * _values._spinSpeed * Time.deltaTime, 0));
            }
        }

        private void Jump()
        {
            _values._rigidbody.AddForce(_values._jumpForce * Vector3.up);
        }
        public void GroundCheck()
        {
            //Collider[] collisions = Physics.OverlapSphere(groundCheck.position, 0.5f, groundMask);
            //if (collisions.Length > 0)
            //{
            //    foreach (var collider in collisions)
            //    {
            //        if (collider.transform.CompareTag("Ground"))
            //        {
            //            ResetJump();
            //        }
            //    }
            //}
        }

        //CameraOffset
        public Transform GiveOffset() => _values._cameraOffset;
        public void SetOffset(Transform a_transform) => _values._cameraOffset = a_transform;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Hackable"))
            {
                _values._currentInteract = other.GetComponent<IPlayerObject>();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Hackable"))
            {
                _values._currentInteract = null;
            }
        }

        private void MoveInputEnter(InputAction.CallbackContext a_context) => _values._moveInput = a_context.ReadValue<Vector2>();
        private void MoveInputExit(InputAction.CallbackContext a_context) => _values._moveInput = Vector2.zero;
        private void SpinInputEnter(InputAction.CallbackContext a_context) => _values._spinInput = a_context.ReadValue<Vector2>();
        private void SpinInputExit(InputAction.CallbackContext a_context) => _values._spinInput = Vector2.zero;
        private void JumpInputEnter(InputAction.CallbackContext a_context) => Jump();
        private void JumpInputExit(InputAction.CallbackContext a_context) => _values._holdingJump = false;
            
        private void Interact(InputAction.CallbackContext a_context)
        {
            if (_values._currentInteract != null)
            {
                /*
                 * Make a dot product check to enable the lights and etc ("possibly change to update method")
                 */
                
                PlayerController.PlayerControl.SwapPlayer(_values._currentInteract);
            }
        }

        private void EnableInput()
        {
            GlobalData.InputManager.Player.Enable();
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled += JumpInputExit;
            GlobalData.InputManager.Player.Camera.performed += SpinInputEnter;
            GlobalData.InputManager.Player.Camera.canceled += SpinInputExit;
            GlobalData.InputManager.Player.Interaction.performed += Interact;
        }

        private void DisableInput()
        {
            GlobalData.InputManager.Player.Disable();
            GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed -= JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled -= JumpInputExit;
            GlobalData.InputManager.Player.Camera.performed -= SpinInputEnter;
            GlobalData.InputManager.Player.Camera.canceled -= SpinInputExit;
            GlobalData.InputManager.Player.Interaction.performed -= Interact;
        }
        
    }
}