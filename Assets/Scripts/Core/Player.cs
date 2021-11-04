using System.Collections;
using System.Collections.Generic;
using Malicious.GameItems;
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
        [SerializeField] private float _fanMoveSpeedScale = 0.5f;
        [SerializeField] private float _spinSpeedModel = 5f;
        [SerializeField] private float _slowDownSpeed = 0.85f;
        //-------------------------------------//
        
        //Camera Variables//
        public static float _spinSpeedCamX = 5f;
        public static float _spinSpeedCamY = 5f;
        public static bool _invertCamX = false;
        public static bool _invertCamY = false;

        [SerializeField] private float _minXCameraAngle = 5f;
        [SerializeField] private float _maxXCameraAngle = 90f;

        [SerializeField] private Transform _cameraOffset = null;
        [SerializeField] private float _cameraAngleMin = 1f;
        //-------------------------------------//
        
        //Animator Variables//
        [SerializeField] private float _animationSwapSpeed = 3f;
        [SerializeField] private Animator _playerAnimator = null;
        private readonly int _animatorRunVariable = Animator.StringToHash("RunAmount");
        private static readonly int _Jumped = Animator.StringToHash("Jumped");
        private static readonly int _Falling = Animator.StringToHash("Falling");
        private static readonly int _Landed = Animator.StringToHash("Landed");
        private static readonly int _ResetToRun = Animator.StringToHash("ResetToRun");
        //[SerializeField] private float _jumpDuration = 1.5f;
        //[SerializeField] private float _landResetDuration = 1.5f;
        //[SerializeField] private float _landDuration = 1.5f;
        
        //private readonly int _jumpingVariable = Animator.StringToHash("Jumping");
        //private float _currentRunAmount = 0f;
        private float _prevRunAnimAmount = 0;
        //--------------------------------//
        
        
        //Input Variables//
        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _spinInput = Vector2.zero;

        //-------------------------------------//
        
        
        //Jumping Variables//
        [SerializeField] private float _maxVelocityToJump = 0.6f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _additionalGravity = -9.81f;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private Transform _groundCheck = null;
        [SerializeField] private float _groundCheckAngleAllowance = 0.7f;
        private bool _canJump = true;
        private bool _hasDoubleJumped = false;
        private bool _holdingJump = false;
        private bool _isJumping = false;
        //private bool _isJumping = false;

        [SerializeField] private float _groundCheckDelay = 0.2f;
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

        //Fan Variables//
        private bool _inFanUp = false;
        private bool _inFanHoriz = false;
        private int _amountOfUpFans = 0;
        private int _amountOfHozFans = 0;
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
            if (_currentHackableField == a_field)
                return;

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

                if (Mathf.Abs(distanceToCurrent - distanceToNew) < 2)
                {
                    if (_currentHackableField.transform.position.y > a_field.transform.position.y)
                        _currentHackableField = a_field;
                }
                else if (distanceToNew < distanceToCurrent)
                    _currentHackableField = a_field;
            }
            else
                _currentHackableField = a_field;
        }
        private void Start()
        {
            if (_inFanUp)
                transform.position += Vector3.zero;

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
            GameEventManager.PlayerStopInput += PlayerDied;
            GameEventManager.PlayerDead += PlayerDead;

            _spinSpeedCamX = GlobalData._cameraSettings.CameraXSpeed;
            _spinSpeedCamY = GlobalData._cameraSettings.CameraYSpeed;
            _invertCamX = GlobalData._cameraSettings.InvertX;
            _invertCamY = GlobalData._cameraSettings.InvertY;
        }
        private void Tick()
        {
            UpdateAnimator();
            SpinMovement();
        }
        private void FixedTick()
        {
            Movement();
            GroundCheck();
            JumpManagement();
        }
        public void LaunchPlayer(Vector3 a_force)
        {
            _rigidbody.velocity = a_force;
        }
        private void JumpManagement()
        {
            if (_isJumping)
            {
                if (_rigidbody.velocity.y < 0.2f)
                {
                    _playerAnimator.SetBool(_Jumped, false);
                    _playerAnimator.SetBool(_Falling, true);
                }
            }
        }

        private void SpinMovement()
        {
            if (_spinInput != Vector2.zero)
            {
                if (_invertCamX && _spinInput.x != 0)
                {
                    Vector3 rotationAmount = new Vector3(0, _spinInput.x * -_spinSpeedCamX * Time.deltaTime, 0);
                    _cameraOffset.Rotate(rotationAmount, Space.World);
                }
                else if (_spinInput.x != 0)
                {
                    Vector3 rotationAmount = new Vector3(0, _spinInput.x * _spinSpeedCamX * Time.deltaTime, 0);
                    _cameraOffset.Rotate(rotationAmount, Space.World);
                }

                if (_invertCamY && _spinInput.y != 0)
                {
                    Vector3 eularAmount = _cameraOffset.rotation.eulerAngles;
                    eularAmount.x += -_spinInput.y * _spinSpeedCamY * Time.deltaTime;
                    eularAmount.x = ClampAngle(eularAmount.x, _minXCameraAngle, _maxXCameraAngle);
                    _cameraOffset.rotation = Quaternion.Euler(eularAmount);
                }
                else if (_spinInput.y != 0)
                {
                    Vector3 eularAmount = _cameraOffset.rotation.eulerAngles;
                    eularAmount.x += _spinInput.y * _spinSpeedCamY * Time.deltaTime;
                    eularAmount.x = ClampAngle(eularAmount.x, _minXCameraAngle, _maxXCameraAngle);
                    _cameraOffset.rotation = Quaternion.Euler(eularAmount);
                }
            }
        }
        private float ClampAngle(float a_angle, float a_min, float a_max)
        {
            if (a_angle < 90 || a_angle > 270)
            {      
                if (a_angle > 180) a_angle -= 360; 
                if (a_max > 180) a_max -= 360;
                if (a_min > 180) a_min -= 360;
            }
            a_angle = Mathf.Clamp(a_angle, a_min, a_max);
            if (a_angle < 0) a_angle += 360;  
            return a_angle;
        }

public void EnteredFan(bool a_isUp)
        {
            _canJump = false;
            _hasDoubleJumped = false;
            
            if (a_isUp)
            {
                _inFanUp = true;
                _amountOfUpFans++;
            }
            else
            {
                _amountOfHozFans++;
                _inFanHoriz = true;
            }
        }

        public void ExitedFan(bool a_isUp)
        {
            if (a_isUp)
            {
                _amountOfUpFans--;
                if (_amountOfUpFans <= 0)
                {
                    //redundancy setting to 0 just in case
                    _amountOfUpFans = 0;
                    _inFanUp = false;
                }
            }
            else
            {
                _amountOfHozFans--;
                if (_amountOfHozFans <= 0)
                {
                    _amountOfHozFans = 0;
                    _inFanHoriz = false;
                }
            }
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
            CameraController.ChangeCamera(ObjectType.Player, _cameraOffset);
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
                Vector2 normalisedInput = _moveInput.normalized;

                Vector3 movementDirection = _cameraOffset.transform.forward * normalisedInput.y + _cameraOffset.transform.right * normalisedInput.x;
                movementDirection.y = 0;
                movementDirection = movementDirection.normalized;

                float angleFromForward = Vector3.SignedAngle(transform.forward, movementDirection, Vector3.up);

                if (Mathf.Abs(angleFromForward) > _cameraAngleMin)
                {
                    transform.Rotate(0, angleFromForward * Time.deltaTime * _spinSpeedModel, 0);

                    _cameraOffset.transform.Rotate(0, -angleFromForward * Time.deltaTime * _spinSpeedModel, 0, Space.World);
                }
                

                float scaleAmount = _moveInput.magnitude;
                
                if (scaleAmount > 1)
                    scaleAmount = 1;
                
                float currentYAmount = _rigidbody.velocity.y;

                
                Vector3 newVel =
                    _cameraTransform.forward * (normalisedInput.y * _moveSpeed * Time.deltaTime) +
                    _cameraTransform.right * (normalisedInput.x * _moveSpeed * Time.deltaTime);
                
                //We are checking if the horizontal speed is too great 
                Vector3 tempVelocity = _rigidbody.velocity + newVel;
                tempVelocity.y = 0;
                if (_inFanHoriz)
                    tempVelocity *= _fanMoveSpeedScale;

                float scaledMaxSpeed = _maxSpeed * scaleAmount;
                bool greaterThanMax = false;
                if (tempVelocity.magnitude > scaledMaxSpeed)
                {
                    greaterThanMax = true;
                    tempVelocity = tempVelocity.normalized * scaledMaxSpeed;
                }

                if (greaterThanMax && _inFanHoriz)
                    return;

                tempVelocity.y = currentYAmount;
                _rigidbody.velocity = tempVelocity;
            }

            if (Mathf.Abs(_moveInput.magnitude) < 0.1f && !_inFanHoriz)
            {
                Vector3 newVel = _rigidbody.velocity;
                //if we are actually moving 
                if (Mathf.Abs(_rigidbody.velocity.x) > 0 || Mathf.Abs(_rigidbody.velocity.z) > 0)
                {
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * _slowDownSpeed;
                    newVel.x = newVel.x * _slowDownSpeed;
                    _rigidbody.velocity = newVel;
                }
                newVel.y = 0;
                if (newVel.sqrMagnitude < 0.2f)
                {
                    newVel = Vector3.zero;
                    newVel.y = _rigidbody.velocity.y;
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
            EnableInput();
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
            if ((_canJump || _hasDoubleJumped == false) && _rigidbody.velocity.y < _maxVelocityToJump)
            {
                StartCoroutine(JumpWait());
                Vector3 prevVel = _rigidbody.velocity;
                prevVel.y = _jumpForce;
                _rigidbody.velocity = prevVel;

                _playerAnimator.SetBool(_Jumped, true);

                if (_canJump == false)
                    _hasDoubleJumped = true;
                
                _canJump = false;
                _isJumping = true;
            }
        }

        private void GroundCheck()
        {
            Collider[] collisions = Physics.OverlapSphere(_groundCheck.position, 0.5f, _groundMask);
            if (collisions == null || collisions.Length == 0)
                _canJump = false;
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

        private void PlayerDied()
        {
            DisableInput();
            _moveInput = Vector2.zero;
        }
        #region Collisions
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.CompareTag("DamagePlayerNoKnockback") && _iFrameActive == false)
            {
                GameEventManager.PlayerHitFunc();
                StartCoroutine(IFrame());
            }
            else if ((other.collider.gameObject.CompareTag("Enemy") || 
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
                _isJumping = false;
                _canJump = true;
                _hasDoubleJumped = false;
            }
            else
            {
                Ray ray = new Ray(_groundCheck.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1, _groundMask))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > _groundCheckAngleAllowance)
                    {
                        _isJumping = false;
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

                if (_activeCheckpoint != null)
                    _activeCheckpoint.TurnOff();

                _activeCheckpoint = currentCheckPoint;
                _activeCheckpoint.TurnOn();

                //if (_activeCheckpoint == null || _activeCheckpoint._ID < currentCheckPoint._ID)
                //{
                //}
            }

            if (a_other.gameObject.CompareTag("Laser"))
            {
                LaunchPlayer(_rigidbody.velocity = a_other.gameObject.GetComponent<BrokenWire>().DirectionToHit(transform.position) * _hitForce);
                
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
            
            yield return new WaitForSeconds(_groundCheckDelay);
            
        }
        private IEnumerator DisableMoveInput()
        {
            _movementDisabled = true;
            yield return new WaitForSeconds(_iframeTime * 0.50f);
            _movementDisabled = false;
        }
        
        #region Input

        private bool _heldInputDown;

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
        private void CameraInputEnter(InputAction.CallbackContext a_context)
        {
            _spinInput = a_context.ReadValue<Vector2>();
        }
        private void CameraInputExit(InputAction.CallbackContext a_context)
        {
            _spinInput = Vector2.zero;
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
            GlobalData.InputManager.Player.Camera.performed += CameraInputEnter;
            GlobalData.InputManager.Player.Camera.canceled += CameraInputExit;
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
            GlobalData.InputManager.Player.Camera.performed -= CameraInputEnter;
            GlobalData.InputManager.Player.Camera.canceled -= CameraInputExit;
            //GlobalData.InputManager.Player.Down.performed -= DownInputEnter;
            //GlobalData.InputManager.Player.Down.canceled -= DownInputExit;
        }
        #endregion
    }
}