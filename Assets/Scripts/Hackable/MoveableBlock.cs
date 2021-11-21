using System;
using Malicious.Core;
using Malicious.GameItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Malicious.Interactables;

namespace Malicious.Hackable
{
    [RequireComponent(typeof(HackableField))]
    public class MoveableBlock : BasePlayer
    {
        [SerializeField] private Transform _cameraOffset = null;

        private bool _isSpring = false;
        [SerializeField] private bool _faceBoxOnExit = true;
        [SerializeField] private Transform _exitPosition = null;
        [SerializeField] private Collider _exitBox = null;
        [SerializeField] private Vector3 _exitDirection = Vector3.zero;
        [SerializeField] private float _exitForce = 4f;
        [SerializeField] private float _dotAllowanceForStacking = 0.7f;
        [SerializeField] private Transform _stackingArea = null;
        [SerializeField] private float _slowDownSpeed = 0.85f;
        [SerializeField] private Transform _rampCheck = null;
        [SerializeField] private float _yAngle = -2f;
        [SerializeField] private float _rampCheckDistance = 3f;
        [SerializeField] private float _rampAngleAllowance = 0.5f;

        [SerializeField] private float _maxYDifferenceRamp = 0.09f;
        
        [SerializeField] private LayerMask _rampMask = ~0;
        [SerializeField] private UnityEvent _onHackEnterEvent = null;
        [SerializeField] private UnityEvent _onHackExitEvent = null;
        private Vector3 _startingPosition = Vector3.zero;
        private GameObject _stackedObject = null;
        public bool _onPressurePlate = false;
        public static bool _invertCamX = false;
        public static float _spinSpeedCamX = 5f;
        //public static float _spinSpeedCamY = 5f;

        public override void Start()
        {
            base.Start();
            _rigidbody = GetComponent<Rigidbody>();
            _cameraTransform = _cameraOffset;
            _startingPosition = transform.position;
            _invertCamX = GlobalData._cameraSettings.InvertX;
            _spinSpeedCamX = GlobalData._cameraSettings.CameraXSpeed;

            if (GetComponent<Spring>() != null)
                _isSpring = true;
        }

        protected override void Tick()
        {
            SpinMovement();
        }

        protected override void FixedTick()
        {
            Movement();
        }
        private void Movement()
        {
            if (_moveInput != Vector2.zero)
            {
                Vector3 camForward = _cameraOffset.forward;
                camForward.y = 0;
                camForward = camForward.normalized;
                Vector3 camRight = _cameraOffset.right;
                camRight.y = 0;
                camRight = camRight.normalized;

                Vector3 newVel =
                    camForward * (_moveInput.y * _moveSpeed * Time.deltaTime) +
                    camRight * (_moveInput.x * _moveSpeed * Time.deltaTime);

                
                _rigidbody.velocity += newVel;

                Vector3 currentVel = _rigidbody.velocity;
                float currentY = currentVel.y;
                currentVel.y = 0;
                if (Vector3.SqrMagnitude(currentVel) > _maxSpeed && !_inFanHoriz)
                {
                    currentVel = currentVel.normalized * _maxSpeed;
                    currentVel.y = currentY;
                    _rigidbody.velocity = currentVel;
                }

                Vector3 checkDirection = currentVel;
                checkDirection.y = 0;
                checkDirection = checkDirection.normalized;
                checkDirection.y = _yAngle;
                Ray ray = new Ray(_rampCheck.position, checkDirection);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _rampCheckDistance, _rampMask))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > _rampAngleAllowance)
                    {
                        if (Mathf.Abs(transform.position.y - hit.point.y) < _maxYDifferenceRamp)
                        {


                            Vector3 currentPos = transform.position;

                            float yHit = hit.point.y;
                            if (hit.point.y > transform.position.y)
                                currentPos.y = yHit;

                            transform.position = currentPos;
                        }
                    }
                }
            }
            
            if (Mathf.Abs(_moveInput.magnitude) < 0.1f && !_inFanHoriz)
            {
                //if we are actually moving 
                if (Mathf.Abs(_rigidbody.velocity.x) > 0f || Mathf.Abs(_rigidbody.velocity.z) > 0)
                {
                    Vector3 adjustedVel = _rigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    adjustedVel.z = adjustedVel.z * _slowDownSpeed;
                    adjustedVel.x = adjustedVel.x * _slowDownSpeed;
                    _rigidbody.velocity = adjustedVel;
                }
                Vector3 currentVel = _rigidbody.velocity;

                currentVel.y = 0;
                if (currentVel.sqrMagnitude < 0.2f)
                {
                    currentVel = Vector3.zero;
                    currentVel.y = _rigidbody.velocity.y;
                    _rigidbody.velocity = currentVel;
                }
            }
        }

        public void ResetToOriginalPosition()
        {
            transform.position = _startingPosition;
        }

        private void SpinMovement()
        {
            if (_spinInput != Vector2.zero)
            {
                if (_invertCamX)
                {
                    _cameraOffset.RotateAround(transform.position, Vector3.up,
                    _spinInput.x * -_spinSpeedCamX * Time.deltaTime);
                }
                else
                {
                    _cameraOffset.RotateAround(transform.position, Vector3.up,
                        _spinInput.x * _spinSpeedCamX * Time.deltaTime);
                }
            }
        }

        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            if (!_canExit)
                return;

            //exit out
            _player.transform.position = _exitPosition.position;
            Vector3 rotationDirection = Vector3.zero;
            
            if (_faceBoxOnExit) 
                rotationDirection = transform.position - _exitPosition.position;
            else
                rotationDirection = _exitPosition.position - transform.position;

            rotationDirection.y = 0;
            rotationDirection = rotationDirection.normalized;
            
            _player.transform.rotation = Quaternion.LookRotation(rotationDirection);
            _player.OnHackEnter();
            _player.LaunchPlayer(_exitDirection * _exitForce);
            OnHackExit();
        }

        public override void OnHackEnter()
        {
            _onHackEnterEvent?.Invoke();
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _exitBox.enabled = true;
            base.OnHackEnter();
            
            if (!_isSpring)
                CameraController.ChangeCamera(ObjectType.Moveable, _cameraOffset);
            else
                CameraController.ChangeCamera(ObjectType.Spring, _cameraOffset);
            //Check the materials as well for hack indication
        }
        public override void OnHackExit()
        {
            _rigidbody.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                _onHackExitEvent?.Invoke();
                base.OnHackExit();
            _exitBox.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.gameObject.CompareTag("Laser"))
            //{
            //    _rigidbody.velocity = other.gameObject.GetComponent<BrokenWire>().DirectionToHit(transform.position) * _hitForce;
            //}
            if (other.gameObject.CompareTag("Block") && !other.isTrigger)
            {
                Vector3 directionToObject =
                    (other.gameObject.transform.position - transform.position).normalized;

                if (_stackingArea != null)
                {

                    if (Vector3.Dot(directionToObject, Vector3.up) > _dotAllowanceForStacking &&
                        Vector3.Distance(other.gameObject.transform.position, _stackingArea.transform.position) < 3f)
                    {
                        other.transform.parent = transform;
                        _stackedObject = other.gameObject;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _stackedObject)
                _stackedObject.transform.parent = null;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_exitPosition != null)
                Gizmos.DrawLine(_exitPosition.position, (_exitPosition.position + _exitDirection * 4f));
            
            Vector3 directionOfLine = Vector3.forward;
            directionOfLine.y = _yAngle;
            Gizmos.DrawLine(_rampCheck.position, _rampCheck.position + directionOfLine * _rampCheckDistance);
            //draw exit velocities and etc

            if (_rigidbody != null)
            {
                Vector3 checkDirection = _rigidbody.velocity;
                checkDirection.y = 0;
                checkDirection = checkDirection.normalized;
                checkDirection.y = _yAngle;
                Gizmos.DrawLine(_rampCheck.position, _rampCheck.position + checkDirection);
            }

        }
#endif
    }
}