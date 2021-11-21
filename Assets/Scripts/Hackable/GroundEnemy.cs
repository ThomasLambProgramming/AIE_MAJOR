using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Malicious.Core;
using UnityEditor;
using Malicious.GameItems;

namespace Malicious.Hackable
{
    [SelectionBase]
    public class GroundEnemy : BasePlayer
    {
        [SerializeField] private bool _canHuntPlayer = true;
        [SerializeField] private float _playerSpeed = 10f;
        [SerializeField] private float _playerMaxSpeed = 0;
        [SerializeField] private float _playerRotateSpeed = 0;
        
        [SerializeField] private float _aiRotateSpeed = 10f;
        [SerializeField] private float _maxSteeringForce = 10f;
        [SerializeField] private float _goNextDistance = 4;
        [SerializeField] private float _angleOfVision = 45f;
        [SerializeField] private List<Vector3> _groundPath = new List<Vector3>();
        [SerializeField] private int _pathIndex = 0;
        [SerializeField] private Transform _exitLocation = null;
        [SerializeField] private float _exitForce = 5f;
        [SerializeField] private float _waitTimeOnExit = 1.5f;
        [SerializeField] private float _hitForce = 4f;

        [SerializeField] private Material _hackedMaterial = null;
        [SerializeField] private Material _defaultMaterial = null;

        [SerializeField] private MeshRenderer _wheelLeft = null;
        [SerializeField] private MeshRenderer _wheelRight = null;
        [SerializeField] private MeshRenderer _orbRenderer = null;
        [SerializeField] private MeshRenderer _mainBody = null;

        [SerializeField] private Transform _leftWheelTransform = null;
        [SerializeField] private Transform _rightWheelTransform = null;

        private int direction = 1;
        private bool _huntPlayer = false;
        private GameObject _playerObject = null;
        private float _sqrMaxSteeringForce = 0;
        private bool _wait = false;
        private Vector3 _directionToTarget = Vector3.zero;
        private Vector3 _startingPosition = Vector3.zero;
        private Quaternion _startingRotation = Quaternion.identity;
        public void Start()
        {
            _hackField = GetComponent<HackableField>();
            GameEventManager.EnemyFixedUpdate += AiUpdate;
            _rigidbody = GetComponent<Rigidbody>();
            _sqrMaxSteeringForce = _maxSteeringForce * _maxSteeringForce;
            _startingPosition = transform.position;
            _startingRotation = transform.rotation;

            
        }

        void AiUpdate()
        {
            if (_wait)
                return;
            
            if (_huntPlayer && _canHuntPlayer)
            {
                if (!_playerObject.activeInHierarchy)
                {
                    _huntPlayer = false;
                    _pathIndex = FindClosestPoint();
                    return;
                }
                if (Mathf.Abs(transform.position.y - _playerObject.transform.position.y) > 3f)
                {
                    _huntPlayer = false;
                    _pathIndex = FindClosestPoint();
                }
                SeekMovement(_playerObject.transform.position);
            }
            else
            {
                if (_groundPath.Count == 0)
                    return;
                
                Vector3 directionToTarget = _groundPath[_pathIndex] - transform.position;
                if (Vector3.SqrMagnitude(directionToTarget) > _goNextDistance)
                {
                    SeekMovement(_groundPath[_pathIndex]);
                }
                else
                {
                    if (direction == 1)
                    {
                        if (_pathIndex < _groundPath.Count - 1)
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
        }

        private void SeekMovement(Vector3 a_target)
        {
            _directionToTarget = a_target - transform.position;

            _directionToTarget.y = 0;
            _directionToTarget = _directionToTarget.normalized;

            Vector3 desiredVelocity = _directionToTarget * _maxSteeringForce;
            Vector3 steeringForce = desiredVelocity - _rigidbody.velocity;

            if (steeringForce.sqrMagnitude > _sqrMaxSteeringForce)
            {
                steeringForce = steeringForce.normalized * _maxSteeringForce;
            }

            _rigidbody.velocity += steeringForce * Time.deltaTime;

            Vector3 currentVel = _rigidbody.velocity;
            float currentY = currentVel.y;
            currentVel.y = 0;
            if (currentVel.magnitude > _maxSpeed && !_inFanHoriz)
            {
                currentVel = currentVel.normalized * _maxSpeed;
                currentVel.y = currentY;
                _rigidbody.velocity = currentVel;
            }

            Quaternion lookDirection = Quaternion.LookRotation(_directionToTarget);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, _aiRotateSpeed);
        }

        private int FindClosestPoint()
        {
            int closestIndex = 0;
            float distanceSqr = 9999990;
            
            for (int i = 0; i < _groundPath.Count; i++)
            {
                float distance = Vector3.SqrMagnitude(_groundPath[i] - transform.position);
                if (distance < distanceSqr)
                {
                    distanceSqr = distance;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
        protected override void Tick()
        {
        }

        protected override void FixedTick()
        {
            if (_moveInput != Vector2.zero)
            {
                if (Mathf.Abs(_moveInput.x) > 0.1f)
                    transform.Rotate(0, _moveInput.x * _playerRotateSpeed * Time.deltaTime, 0);
                
                if (Mathf.Abs(_moveInput.y) > 0.1f) 
                {
                    _rigidbody.velocity += transform.forward * (_moveInput.y * Time.deltaTime * _playerSpeed);
                }
            }
            if (_moveInput.y == 0 && !_inFanHoriz)
            {
                float currentYVel = _rigidbody.velocity.y;
                Vector3 newVelocity = _rigidbody.velocity;
                newVelocity.y = 0;
                
                newVelocity = newVelocity * 0.98f;
                

                float sqrMagnitude = newVelocity.sqrMagnitude;
                if (sqrMagnitude < 3f)
                {
                    if (sqrMagnitude > 0.5f)
                        newVelocity = newVelocity * 0.90f;
                    else
                        newVelocity = Vector3.zero;
                }

                newVelocity.y = currentYVel;
                _rigidbody.velocity = newVelocity;
            }
            if (_moveInput.x == 0) 
                _rigidbody.angularVelocity = Vector3.zero;


            Vector3 currentVel = _rigidbody.velocity;
            float currentY = currentVel.y;
            currentVel.y = 0;
            if (currentVel.magnitude > _playerMaxSpeed && !_inFanHoriz)
            {
                currentVel = currentVel.normalized * _playerMaxSpeed;
                currentVel.y = currentY;
                _rigidbody.velocity = currentVel;
            }
        }

        public override void OnHackEnter()
        {
            base.OnHackEnter();
            GameEventManager.EnemyFixedUpdate -= AiUpdate;
            CameraController.ChangeCamera(ObjectType.GroundEnemy, _cameraTransform);
            _huntPlayer = false;

            _wheelLeft.material = _hackedMaterial;
            _wheelRight.material = _hackedMaterial;
            _orbRenderer.material = _hackedMaterial;
           
            _mainBody.material = _hackedMaterial;
        }
        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            if (_canExit)
                OnHackExit();
        }
        public override void OnHackExit()
        {
            _wait = true;
            StartCoroutine(ExitWaitTime());
            base.OnHackExit();
            GameEventManager.EnemyFixedUpdate += AiUpdate;
            _player.OnHackEnter();
            _player.LaunchPlayer(_exitLocation.forward * _exitForce);
            _player.transform.parent = null;
            Vector3 exitDirection = _exitLocation.forward;
            exitDirection.y = 0;
            exitDirection = exitDirection.normalized;
            
            _player.transform.rotation = Quaternion.LookRotation(exitDirection);
            _player.transform.position = _exitLocation.position;

            _huntPlayer = false;

            if (_groundPath.Count > 0)
            {
                transform.position = _groundPath[0];
                transform.rotation = Quaternion.LookRotation(_groundPath[1] - transform.position);   
            }
            else
            {
                transform.position = _startingPosition;
                transform.rotation = _startingRotation;
            }
            
            _rigidbody.velocity = Vector3.zero;
            _pathIndex = 1;
            _player = null;

            _wheelLeft.material = _defaultMaterial;
            _wheelRight.material = _defaultMaterial;
            _orbRenderer.material = _defaultMaterial;
           
            _mainBody.material = _defaultMaterial;
        }

        IEnumerator ExitWaitTime()
        {
            yield return new WaitForSeconds(_waitTimeOnExit);
            _wait = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Laser"))
            {
                _rigidbody.velocity = other.gameObject.GetComponent<BrokenWire>().DirectionToHit(transform.position) * _hitForce;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Vector3 directionToPlayer = other.gameObject.transform.position - transform.position;
                if (Vector3.Angle(directionToPlayer, transform.forward) < _angleOfVision)
                {
                    _huntPlayer = true;
                    _playerObject = other.gameObject;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _huntPlayer = false;
                _pathIndex = FindClosestPoint();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var point in _groundPath)
            {
                Gizmos.DrawSphere(point, 0.3f);
            }
        }
        #endif
    }
}
