using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Malicious.Core;
using UnityEditor;

namespace Malicious.Hackable
{
    public class GroundEnemy : BasePlayer
    {
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
        private int direction = 1;
        private bool _huntPlayer = false;
        private GameObject _playerObject = null;
        private float _sqrMaxSteeringForce = 0;
        private bool _wait = false;
        private Vector3 _directionToTarget = Vector3.zero;
        
        void Start()
        {
            GameEventManager.EnemyFixedUpdate += AiUpdate;
            _rigidbody = GetComponent<Rigidbody>();
            _sqrMaxSteeringForce = _maxSteeringForce * _maxSteeringForce;
        }

        void AiUpdate()
        {
            if (_wait)
                return;
            
            if (_huntPlayer)
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

            if (_rigidbody.velocity.magnitude > _maxSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
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
            if (_moveInput.y == 0)
            {
                _rigidbody.velocity = _rigidbody.velocity * 0.98f;

                float sqrMagnitude = _rigidbody.velocity.sqrMagnitude;
                if (sqrMagnitude < 3f)
                {
                    if (sqrMagnitude > 0.5f)
                        _rigidbody.velocity = _rigidbody.velocity * 0.90f;
                    else
                        _rigidbody.velocity = Vector3.zero;
                }
            }
            if (_moveInput.x == 0) 
                _rigidbody.angularVelocity = Vector3.zero;
            
            if (_rigidbody.velocity.magnitude > _playerMaxSpeed) 
                _rigidbody.velocity = _rigidbody.velocity.normalized * _playerMaxSpeed;
        }

        public override void OnHackEnter()
        {
            base.OnHackEnter();
            GameEventManager.EnemyFixedUpdate -= AiUpdate;
            CameraController.ChangeCamera(ObjectType.GroundEnemy, _cameraTransform);
        }
        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
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
            
            transform.position = _groundPath[0];
            _rigidbody.velocity = Vector3.zero;
            _pathIndex = 1;
        }

        IEnumerator ExitWaitTime()
        {
            yield return new WaitForSeconds(_waitTimeOnExit);
            _wait = false;
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
