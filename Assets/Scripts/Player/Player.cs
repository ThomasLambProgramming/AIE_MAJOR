using System;
using System.Collections;
using Malicious.Interfaces;
using Malicious.Core;

using UnityEngine.InputSystem;
using UnityEngine;

namespace Malicious.Player
{
    public class Player : MonoBehaviour, IPlayerObject
    {
        //------Standard Variables-------------//
        //I didnt know what to name these
        private Rigidbody _rigidbody = null;
        //-------------------------------------//
        
        
        //------IFrame Variables---------------//
        [SerializeField] private GameObject _ModelContainer = null;
        [SerializeField] private float _iframeTime = 1.5f;
        private bool _iFrameActive = false;
        private bool _isPaused = false;
        //-------------------------------------//
        
        
        //------Camera Variables---------------//
        private Transform _playerCamera = null;
        [SerializeField] private Transform _cameraOffset = null;
        //-------------------------------------//
        
        
        //------Hacking Variables--------------//
        [SerializeField] private float _dotAllowance = 0.8f;
        private IPlayerObject _currentInteract = null;
        private GameObject _currentInteractObj = null;
        private IHackable _currentHackable = null;
        private GameObject _currentHackableObj = null;
        private bool _canInteract = false;
        private bool _canHackable = false;
        //-------------------------------------//
        
        
        //------Speed Variables----------------//
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _maxSpeed = 4f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _spinSpeed = 5f;
        //-------------------------------------//
        
        
        //------Jumping Variables--------------//
        private bool _canJump = true;
        private bool _hasDoubleJumped = false;
        private bool _holdingJump = false;
        [SerializeField] private Transform _groundCheck = null;
        //-------------------------------------//
        
        
        //------Animator Variables-------------//
        [SerializeField] private float _animationSwapSpeed = 3f;
        private Animator _playerAnimator = null;
        private Vector2 _currentAnimationVector = Vector2.zero;
        private readonly int _xPos = Animator.StringToHash("XPos");
        private readonly int _yPos = Animator.StringToHash("YPos");
        private readonly int _jumping = Animator.StringToHash("Jumping");
        //-------------------------------------//
        
        
        //------Input Variables----------------//
        [HideInInspector] public Vector2 _moveInput = Vector2.zero;
        //-------------------------------------//
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerAnimator = GetComponent<Animator>();
            
            GameEventManager.GamePauseStart += PauseEnter;
            GameEventManager.GamePauseExit += PauseExit;

            _playerCamera = Camera.main.transform;
        }

        public void OnHackEnter()
        {
            EnableInput();
            _currentAnimationVector = Vector2.zero;
            gameObject.SetActive(true);
        }

        public void OnHackExit()
        {
            DisableInput();
            gameObject.SetActive(false);
        }

        public void Tick()
        {
            //Logic
            UpdateAnimator();
            Debug.Log("Test");
        }

        public void FixedTick()
        {
            //Movement and etc
            Movement();
            HackValidCheck();
        }

        private Vector3 _prevVelocity = Vector3.zero;
        private void PauseEnter()
        {
            _playerAnimator.enabled = false;
            _moveInput = Vector2.zero;
            DisableInput();
            _isPaused = true;
            _prevVelocity = _rigidbody.velocity;
            _rigidbody.isKinematic = true;
        }

        private void PauseExit()
        {
            _playerAnimator.enabled = true;
            EnableInput();
            _isPaused = false;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _prevVelocity;
        }

        private IEnumerator IFrame()
        {
                
            float timer = 0;
            int frameCount = 0;
            while (timer < _iframeTime)
            {
                if (_isPaused)
                {
                    //just to make sure when paused its not in inactive state
                    _ModelContainer.SetActive(true);   
                    yield return null;
                }
                
                frameCount++;
                if (frameCount >= 20)
                {
                    frameCount = 0;
                    _ModelContainer.SetActive(!_ModelContainer.activeInHierarchy);
                }
                timer += Time.deltaTime;
                yield return null;
            }

            _ModelContainer.SetActive(true);
            _iFrameActive = false;
        }

        public void FanLaunch(Vector3 a_force)
        {
            _canJump = false;
            _hasDoubleJumped = false;
            Vector3 newVel = _rigidbody.velocity;
            newVel.x += a_force.x;
            newVel.z += a_force.z;
            //this is to ensure that the fan will always propel the player up (and reset the double jump)
            newVel.y = a_force.y;
            _rigidbody.velocity = newVel;
        }
        private void Movement()
        {
            if (_values._moveInput != Vector2.zero)
            {
                /*
                 * 
                 */
                
                
                
                
                //For controller users this will change the max movespeed according to how small their inputs are
                float targetAngle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg +
                                    _playerCamera.transform.rotation.eulerAngles.y;
                
                //Rotate player towards current input
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, targetRotation, _spinSpeed * Time.deltaTime);
                
                
                float scaleAmount = _moveInput.magnitude;
                
                float currentYAmount = _rigidbody.velocity.y;
                
                Vector3 newVel =
                    _playerCamera.transform.forward * (_moveInput.y * _moveSpeed * Time.deltaTime) +
                    _playerCamera.transform.right * (_moveInput.x * _moveSpeed * Time.deltaTime);
                
                //We are checking if the horizontal speed is too great 
                Vector3 tempVelocity = _rigidbody.velocity + newVel;
                tempVelocity.y = 0;

                float scaledMaxSpeed = _maxSpeed * scaleAmount;
                if (tempVelocity.magnitude > scaledMaxSpeed)
                {
                    tempVelocity = tempVelocity.normalized * scaledMaxSpeed;
                }

                tempVelocity.y = currentYAmount;
                _rigidbody.velocity = tempVelocity;
                
            }
            
            if (Mathf.Abs(_moveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(_rigidbody.velocity.x) > 0.2f || Mathf.Abs(_rigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = _rigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * 0.90f;
                    newVel.x = newVel.x * 0.90f;
                    _rigidbody.velocity = newVel;
                }
            }

            Vector3 tempVel = _rigidbody.velocity;
            tempVel.y = 0;
            if (tempVel.sqrMagnitude < 0.1f)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            }
        }
        private void UpdateAnimator()
        {
            _currentAnimationVector = new Vector2(
                Mathf.Lerp(_currentAnimationVector.x, 
                    _moveInput.x, 
                    _animationSwapSpeed * Time.deltaTime),
                Mathf.Lerp(_currentAnimationVector.y, 
                    _moveInput.y, 
                    _animationSwapSpeed * Time.deltaTime));
        
            _playerAnimator.SetFloat(_xPos, _currentAnimationVector.x);
            _playerAnimator.SetFloat(_yPos, _currentAnimationVector.y);
        }
        private void Jump()
        {
            _rigidbody.AddForce(_jumpForce * Vector3.up);
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
        private void HackValidCheck()
        {
            if (_currentHackable != null)
            {
                if (DotCheck(transform, _currentHackableObj.transform))
                {
                    _canHackable = true;
                    _currentHackable.OnHackValid();
                }
                else
                {
                    _canHackable = false;
                    _currentHackable.OnHackFalse();
                }
            }

            if (_currentInteract != null)
            {
                if (DotCheck(transform, _currentInteractObj.transform))
                {
                    _canInteract = true;
                    _currentInteract.OnHackValid();
                }
                else
                {
                    _canInteract = false;
                    _currentInteract.OnHackFalse();
                }
            }
        }
        private bool DotCheck(Transform a_transform, Transform b_transform)
        {
            Vector3 direction = (b_transform.position - a_transform.position).normalized;
            if (Vector3.Dot(direction, a_transform.forward) > _dotAllowance)
            {
                return true;
            }

            return false;
        }

        //for all materials and other graphical changes when the player can hack
        public void OnHackValid(){}
        public void OnHackFalse(){}
        public ObjectType ReturnType() => ObjectType.Player;

        public OffsetContainer GiveOffset()
        {
            OffsetContainer temp = new OffsetContainer();
            temp._offsetTransform = _cameraOffset;
            temp._rigOffset = _rigOffset;
            return temp;
        }
        public void SetOffset(Transform a_transform) => _cameraOffset = a_transform;
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Hackable"))
            {
                if (!_iFrameActive)
                {
                    PlayerController.PlayerControl.PlayerHit();
                    StartCoroutine(IFrame());
                    _iFrameActive = true;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Hackable"))
            {
                _currentInteract = other.GetComponent<IPlayerObject>();
                _currentInteractObj = other.gameObject;
            }
            else if (other.gameObject.CompareTag("Interactable"))
            {
                _currentHackable = other.GetComponent<IHackable>();
                _currentHackableObj = other.gameObject;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Hackable"))
            {
                //Double check that the object gets set back to original state
                if (_currentInteract != null)
                    _currentInteract.OnHackFalse();
                
                _currentInteract = null;
                _currentInteractObj = null;
                _canInteract = false;
            }
            else if (other.gameObject.CompareTag("Interactable"))
            {
                //Double check that the object gets set back to original state
                if (_currentHackable != null)
                    _currentHackable.OnHackFalse();
                
                _currentHackable = null;
                _currentHackableObj = null;
                _canHackable = false;
            }
        }

        private void Interact(InputAction.CallbackContext a_context)
        {
            if (_currentInteract != null && _canInteract)
            {
                PlayerController.PlayerControl.SwapPlayer(_currentInteract);
            }
            else if (_currentHackable != null && _canHackable)
            {
                _currentHackable.Hacked();
            }
        }
        public bool RequiresTruePlayerOffset() => false;
        private void MoveInputEnter(InputAction.CallbackContext a_context) => _moveInput = a_context.ReadValue<Vector2>();
        private void MoveInputExit(InputAction.CallbackContext a_context) => _moveInput = Vector2.zero;
        private void JumpInputEnter(InputAction.CallbackContext a_context) => Jump();
        private void JumpInputExit(InputAction.CallbackContext a_context) => _holdingJump = false;
            

        private void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled += JumpInputExit;
            GlobalData.InputManager.Player.Interaction.performed += Interact;
        }

        private void DisableInput()
        {
            GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed -= JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled -= JumpInputExit;
            GlobalData.InputManager.Player.Interaction.performed -= Interact;
        }
        
    }
}