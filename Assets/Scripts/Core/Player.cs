using System.Collections;
using System.Collections.Generic;
using Malicious.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Core
{
    public class Player : MonoBehaviour
    {
        #region Variables
        
        //Speed Variables//
        [SerializeField] private float _moveSpeed = 100f;
        [SerializeField] private float _maxSpeed = 4f;
        [SerializeField] private float _spinSpeed = 5f;
        //-------------------------------------//
        
        
        //Animator Variables//
        [SerializeField] private float _animationSwapSpeed = 3f;
        [SerializeField] private Animator _playerAnimator = null;
        private readonly int _animatorRunVariable = Animator.StringToHash("RunAmount");
        //private readonly int _jumpingVariable = Animator.StringToHash("Jumping");
        //private float _currentRunAmount = 0f;
        private float _prevRunAnimAmount = 0;
        //--------------------------------//
        
        
        //Input Variables//
        private Vector2 _moveInput = Vector2.zero;
        //-------------------------------------//
        
        
        //Jumping Variables//
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _additionalGravity = -9.81f;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private Transform _groundCheck = null;
        private bool _canJump = true;
        private bool _hasDoubleJumped = false;
        private bool _holdingJump = false;

        [SerializeField] private float _groundCheckDelay = 0.2f;
        private bool _waitingForEnumerator = false;
        //--------------------------------//
        
        
        //IFrame Variables//
        private bool _isPaused = false;
        private bool _iFrameActive = false;
        private bool _movementDisabled = false;
        [SerializeField] private float _hitForce = 4f;
        [SerializeField] private float _iframeTime = 1.5f;
        [SerializeField] private float _yHitAmount = 3f;
        [SerializeField] private GameObject _modelContainer = null;
        //--------------------------------//
        
        
        //Misc Variables That couldnt be grouped
        [SerializeField] private Transform _cameraTransform = null;
        private Rigidbody _rigidbody = null;
        private Vector3 _pauseEnterVelocity = Vector3.zero;
        private HackableField _currentHackableField = null;
        [SerializeField] private CheckPoint _activeCheckpoint = null;
        //--------------------------------//
        #endregion
        public void SetHackableField(HackableField a_field)
        {
            if (a_field == null)
            {
                _currentHackableField = null;
                return;
            }
            if (_currentHackableField != null)
            {
                float distanceToCurrent =
                    Vector3.SqrMagnitude(transform.position - _currentHackableField.transform.position);
                float distanceToNew =
                    Vector3.SqrMagnitude(transform.position - a_field.transform.position);
                if (distanceToNew < distanceToCurrent)
                    _currentHackableField = a_field;
            }
            else
                _currentHackableField = a_field;
        }
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerAnimator = _modelContainer.GetComponent<Animator>();
            _playerAnimator.SetFloat(_animatorRunVariable, 0);
            EnableInput();
            
            GameEventManager.GamePauseStart += PauseEnter;
            GameEventManager.GamePauseExit += PauseExit;

            _cameraTransform = Camera.main.transform;

            GameEventManager.PlayerUpdate += Tick;
            GameEventManager.PlayerFixedUpdate += FixedTick;
            //_currentRunAmount = 0;

            GameEventManager.PlayerDead += PlayerDead;
        }
        private void Tick()
        {
            UpdateAnimator();
        }
        private void FixedTick()
        {
            Movement();
            GroundCheck();
        }
        public void LaunchPlayer(Vector3 a_force)
        {
            _rigidbody.velocity = a_force;
        }
        public void OnHackEnter()
        {
            EnableInput();
            _moveInput = Vector2.zero;
            GameEventManager.PlayerUpdate += Tick;
            GameEventManager.PlayerFixedUpdate += FixedTick;
            //_currentRunAmount = 0;
            _currentHackableField = null;
            gameObject.SetActive(true);
            CameraController.ChangeCamera(ObjectType.Player);
            _heldInputDown = false;
        }
        public void OnHackExit()
        {
            DisableInput();
            _moveInput = Vector2.zero;
            GameEventManager.PlayerUpdate -= Tick;
            GameEventManager.PlayerFixedUpdate -= FixedTick;
            gameObject.SetActive(false);
            _heldInputDown = false;
        }
        private void Movement()
        {
            if (_moveInput != Vector2.zero && !_movementDisabled)
            {
                //For controller users this will change the max movespeed according to how small their inputs are
                float targetAngle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg +
                                    _cameraTransform.rotation.eulerAngles.y;
                
                //Rotate player towards current input
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, targetRotation, _spinSpeed * Time.deltaTime);
                
                
                float scaleAmount = _moveInput.magnitude;
                
                float currentYAmount = _rigidbody.velocity.y;

                Vector2 normalisedInput = _moveInput.normalized;
                
                Vector3 newVel =
                    _cameraTransform.forward * (normalisedInput.y * _moveSpeed * Time.deltaTime) +
                    _cameraTransform.right * (normalisedInput.x * _moveSpeed * Time.deltaTime);
                
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

            if (!_holdingJump && _canJump == false)
            {
                _rigidbody.velocity = new Vector3(
                    _rigidbody.velocity.x,
                    _rigidbody.velocity.y + _additionalGravity * Time.deltaTime,
                    _rigidbody.velocity.z);
            }
        }

        private void PlayerDead()
        {
            transform.position = _activeCheckpoint._returnPosition;
            transform.rotation = Quaternion.LookRotation(_activeCheckpoint._facingDirection);
        }
        #region Pausing
        private void PauseEnter()
        {
            _playerAnimator.enabled = false;
            _moveInput = Vector2.zero;
            DisableInput();
            _isPaused = true;
            _pauseEnterVelocity = _rigidbody.velocity;
            _rigidbody.isKinematic = true;
        }
        private void PauseExit()
        {
            _playerAnimator.enabled = true;
            EnableInput();
            _isPaused = false;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _pauseEnterVelocity;
        }
        #endregion
        #region Jumping
        private void Jump()
        {
            if (_movementDisabled)
                return;
            //the 2 y velocity check is so the player can jump just before the arc of their jump
            if ((_canJump || _hasDoubleJumped == false) && _rigidbody.velocity.y < 2)
            {
                StartCoroutine(JumpWait());
                Vector3 prevVel = _rigidbody.velocity;
                prevVel.y = _jumpForce;
                _rigidbody.velocity = prevVel;
                
               _playerAnimator.SetBool(_Jumping, true);
                
                if (_canJump == false)
                    _hasDoubleJumped = true;
                _canJump = false;
            }
        }

        private void GroundCheck()
        {
            Collider[] collisions = Physics.OverlapSphere(_groundCheck.position, 1f, _groundMask);
            if (collisions.Length == 0)
            {
                _canJump = false;
            }
        }

        #endregion
        private void UpdateAnimator()
        {
            Vector3 vel = _rigidbody.velocity;
            vel.y = 0;
            
            float animatorAmount = 0;
            if (vel.magnitude > 0)
                animatorAmount = vel.magnitude / _maxSpeed;

            _prevRunAnimAmount = Mathf.Lerp(_prevRunAnimAmount, animatorAmount, Time.deltaTime * _animationSwapSpeed);
            
            _playerAnimator.SetFloat(_animatorRunVariable, _prevRunAnimAmount);
        }
        #region Collisions
        private void OnCollisionEnter(Collision other)
        {
            if ((other.gameObject.CompareTag("Enemy") || 
                other.gameObject.CompareTag("Laser")) && 
                _iFrameActive == false)
            {
                List<ContactPoint> contacts = new List<ContactPoint>(); 
                other.GetContacts(contacts);

                Vector3 averagedNormal = Vector3.zero;
                foreach (var contactPoint in contacts)
                {
                    averagedNormal += contactPoint.normal;
                }

                averagedNormal.y = 0;
                averagedNormal = averagedNormal.normalized;
                averagedNormal.y = _yHitAmount;
                LaunchPlayer(averagedNormal * _hitForce);
                
                GameEventManager.PlayerHitFunc();
                if (GameEventManager.CurrentHealth() <= 0)
                {
                    //Run shader for dissolve
                }
                else
                {
                    StartCoroutine(IFrame());
                    StartCoroutine(DisableMoveInput());
                }
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                _canJump = true;
                _hasDoubleJumped = false;
            }
            else if (other.gameObject.CompareTag("Environment"))
            {
                List<ContactPoint> contacts = new List<ContactPoint>(); 
                other.GetContacts(contacts);

                foreach (var contactPoint in contacts)
                {
                    if (Vector3.Dot(contactPoint.normal, Vector3.up) > 0.8f)
                    {
                        _canJump = true;
                        _hasDoubleJumped = false;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider a_other)
        {
            if (a_other.gameObject.CompareTag("CheckPoint"))
            {
                CheckPoint currentCheckPoint = a_other.GetComponent<CheckPoint>();
                
                if (_activeCheckpoint == null || _activeCheckpoint._ID < currentCheckPoint._ID)
                {
                    if (_activeCheckpoint != null)
                        _activeCheckpoint.TurnOff();
                    
                    _activeCheckpoint = currentCheckPoint;
                    _activeCheckpoint.TurnOn();
                }
            }
        }

        #endregion
        private IEnumerator IFrame()
        {
            _iFrameActive = true;
            float timer = 0;
            int frameCount = 0;
            while (timer < _iframeTime)
            {
                if (_isPaused)
                {
                    //just to make sure when paused its not in inactive state
                    _modelContainer.SetActive(true);   
                    yield return null;
                }
                
                frameCount++;
                if (frameCount >= 20)
                {
                    frameCount = 0;
                    _modelContainer.SetActive(!_modelContainer.activeInHierarchy);
                }
                timer += Time.deltaTime;
                yield return null;
            }

            _modelContainer.SetActive(true);
            _iFrameActive = false;
        }

        //this delays the ground check for a short period of time so on jump it doesnt instantly reset
        private IEnumerator JumpWait()
        {
            _waitingForEnumerator = true;
            yield return new WaitForSeconds(_groundCheckDelay);
            _waitingForEnumerator = false;
        }

        private IEnumerator DisableMoveInput()
        {
            _movementDisabled = true;
            yield return new WaitForSeconds(_iframeTime * 0.50f);
            _movementDisabled = false;
        }
        
        #region Input

        private bool _heldInputDown;
        private static readonly int _Jumping = Animator.StringToHash("Jumping");

        private void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            _heldInputDown = true;
            if (_currentHackableField != null)
            {
                _currentHackableField.HackInputStarted();
            }
        }
        private void InteractionInputExit(InputAction.CallbackContext a_context)
        {
            if (_currentHackableField != null && _heldInputDown)
            {
                _currentHackableField.HackInputStopped();
                _heldInputDown = false;
            }
        }
        private void JumpInputEnter(InputAction.CallbackContext a_context)
        {
            if (!_holdingJump)
            {
                Jump();
                _holdingJump = true;
            }
        }
        private void JumpInputExit(InputAction.CallbackContext a_context) => _holdingJump = false;
        private void MoveInputEnter(InputAction.CallbackContext a_context)
        {
            _moveInput = a_context.ReadValue<Vector2>();
        }
        private void MoveInputExit(InputAction.CallbackContext a_context)
        {
            _moveInput = Vector2.zero;
        }
        
        private void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled += JumpInputExit;
            GlobalData.InputManager.Player.Interaction.performed += InteractionInputEnter;
            GlobalData.InputManager.Player.Interaction.canceled += InteractionInputExit;
            
            //left these just incase i added any controls because retyping them is annoying
            //GlobalData.InputManager.Player.Camera.performed += CameraInputEnter;
            //GlobalData.InputManager.Player.Camera.canceled += CameraInputExit;
            //GlobalData.InputManager.Player.Down.performed += DownInputEnter;
            //GlobalData.InputManager.Player.Down.canceled += DownInputExit;
        }
        private void DisableInput()
        {
            GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed -= JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled -= JumpInputExit;
            GlobalData.InputManager.Player.Interaction.performed -= InteractionInputEnter;
            GlobalData.InputManager.Player.Interaction.canceled -= InteractionInputExit;
            //GlobalData.InputManager.Player.Camera.performed -= CameraInputEnter;
            //GlobalData.InputManager.Player.Camera.canceled -= CameraInputExit;
            //GlobalData.InputManager.Player.Down.performed -= DownInputEnter;
            //GlobalData.InputManager.Player.Down.canceled -= DownInputExit;
        }
        #endregion
    }
}