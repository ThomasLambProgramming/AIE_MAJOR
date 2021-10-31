using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using Malicious.GameItems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Hackable
{
    public class FlyingEnemy : BasePlayer
    {
        [SerializeField] private List<Vector3> _flightPath = new List<Vector3>();
        [SerializeField] private int _pathIndex = 0;
        private int direction = 1;
        [SerializeField] private float _goNextDistance = 4;
        [SerializeField] private float _maxTurningSpeed = 10f;
        
        [SerializeField] private float _playerRotateSpeed = 0;
        [SerializeField] private float _exitForce = 2;
        [Tooltip("Exit location z (blue one) is used for the direction to launch player")]
        [SerializeField] private Transform _exitLocation = null;

        [SerializeField] private float _resetWaitTime = 4f;
        [SerializeField] private float _hitForce = 5;
        private bool _isHacked = false;
        private GameObject _playerObject = null;
        private float _sqrMaxTurningSpeed = 0;

        [SerializeField] private LayerMask _raycastMask = ~0;

        private bool _wait = false;
        // Start is called before the first frame update
        void Start()
        {
            GameEventManager.EnemyFixedUpdate += AiUpdate;
            _rigidbody = GetComponent<Rigidbody>();
            _sqrMaxTurningSpeed = _maxTurningSpeed * _maxTurningSpeed;
        }

        void AiUpdate()
        {
            if (_wait)
                return;
            
            if (_playerObject != null && 
                Vector3.SqrMagnitude(transform.position - _playerObject.transform.position) > 9)
            {
                _playerObject.transform.parent = null;
                _playerObject = null;
            }
            
            if (_flightPath.Count == 0)
                return;
            
            Vector3 directionToTarget = _flightPath[_pathIndex] - transform.position;
            if (Vector3.SqrMagnitude(directionToTarget) > _goNextDistance)
            {
                Vector3 desiredVelocity = Vector3.Normalize(directionToTarget) * _maxSpeed;
                Vector3 steeringForce = desiredVelocity - _rigidbody.velocity;

                if (steeringForce.sqrMagnitude > _sqrMaxTurningSpeed)
                {
                    steeringForce = steeringForce.normalized * _maxTurningSpeed;
                }

                _rigidbody.velocity += steeringForce * Time.deltaTime;
                
                if (_rigidbody.velocity.magnitude > _maxSpeed)
                {
                    _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
                }
                Quaternion lookDirection = Quaternion.LookRotation(_rigidbody.velocity.normalized);

                if (transform.rotation != lookDirection)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, _maxTurningSpeed);

                    //if (_playerObject != null)
                    //    _playerObject.transform.RotateAround(transform.position, Vector3.up, _maxTurningSpeed);
                }
            }
            else
            {
                if (direction == 1)
                {
                    if (_pathIndex < _flightPath.Count - 1)
                        _pathIndex++;
                    else
                    {
                        direction = -1;
                        _pathIndex--;
                    }
                }
                else
                {
                    if (_pathIndex > 0)
                        _pathIndex--;
                    else
                    {
                        direction = 1;
                        _pathIndex++;
                    }
                }
            }
        }

        protected override void Tick()
        {
        }

        protected override void FixedTick()
        {
            if (_moveInput != Vector2.zero)
            {
                if (Mathf.Abs(_moveInput.x) > 0.1f)
                {
                    transform.Rotate(0, _moveInput.x * _playerRotateSpeed * Time.deltaTime, 0);
                }
                
                if (Mathf.Abs(_moveInput.y) > 0.1f) 
                {
                    _rigidbody.velocity += transform.forward * (_moveInput.y * Time.deltaTime * _maxSpeed);
                }
            }
            
            if (_rigidbody.velocity.magnitude > _maxSpeed && !_inFanHoriz) 
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;

            if (_moveInput.y == 0 && !_inFanHoriz)
            {
                Vector3 newVel = _rigidbody.velocity;
                float yVel = newVel.y;

                newVel.y = 0;
                newVel = newVel * 0.98f;

                float sqrMagnitude = newVel.sqrMagnitude;
                if (sqrMagnitude < 3f)
                {
                    if (sqrMagnitude > 0.5f)
                        newVel = newVel * 0.90f;
                    else
                        newVel = Vector3.zero;
                }
                newVel.y = yVel;
                _rigidbody.velocity = newVel;
            }
            if (_moveInput.x == 0) 
                _rigidbody.angularVelocity = Vector3.zero;
        }

        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            OnHackExit();
        }

        public override void OnHackEnter()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            base.OnHackEnter();
            _isHacked = true;
            GameEventManager.EnemyFixedUpdate -= AiUpdate;
            CameraController.ChangeCamera(ObjectType.FlyingEnemy, _cameraTransform);
        }

        public override void OnHackExit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            base.OnHackExit();
            GameEventManager.EnemyFixedUpdate += AiUpdate;
            _player.OnHackEnter();
            _player.LaunchPlayer(_exitLocation.forward * _exitForce);
            _player.transform.parent = null;
            _isHacked = false;
            Vector3 exitDirection = _exitLocation.forward;
            exitDirection.y = 0;
            exitDirection = exitDirection.normalized;
            
            _player.transform.rotation = Quaternion.LookRotation(exitDirection);
            _player.transform.position = _exitLocation.position;

            StartCoroutine(WaitToReset());
        }

        private IEnumerator WaitToReset()
        {
            _wait = true;
            yield return new WaitForSeconds(_resetWaitTime);
            transform.position = _flightPath[0];
            _rigidbody.velocity = Vector3.zero;
            _pathIndex = 1;
            _wait = false;
        }
        public bool isHacked()
        {
            return _isHacked;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Fan"))
            {
            }

            if (other.gameObject.CompareTag("Laser"))
            {
                _rigidbody.velocity = other.gameObject.GetComponent<BrokenWire>().DirectionToHit(transform.position) * _hitForce;
            }
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.parent = transform;
                _playerObject = other.gameObject;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.parent = null;
                _playerObject = null;
            }
        }

        //private void OnCollisionEnter(Collision other)
        //{
        //    //if (other.gameObject.CompareTag("Laser"))
        //    //{
        //    //    Vector3 movementDirection = (other.transform.position - transform.position).normalized;
        //    //    Ray ray = new Ray(transform.position, movementDirection);
        //    //    RaycastHit hit;
        //    //    Vector3 forceDirection = Vector3.zero;
        //    //    if (Physics.Raycast(ray, out hit, 10, _raycastMask))
        //    //    {
        //    //        forceDirection = hit.normal;
        //    //    }
        //    //    forceDirection.y = 0;
        //    //    forceDirection = forceDirection.normalized;
        //    //    _rigidbody.velocity = (forceDirection * _hitForce);
        //    //}
        //}

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var point in _flightPath)
            {
                Gizmos.DrawSphere(point, 0.3f);
            }
        }
        #endif
    }
}
