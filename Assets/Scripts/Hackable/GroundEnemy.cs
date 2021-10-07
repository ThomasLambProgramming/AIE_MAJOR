using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Malicious.Core;
using UnityEditor;

namespace Malicious.Hackable
{
    [SelectionBase]
    public class GroundEnemy : BasePlayer
    {
        [SerializeField] private float _turningSpeed = 10f;
        
        [Tooltip("The turning positions for the left and right wheel (like wheel axles)")]
        [SerializeField] private Transform _leftAnchorPoint = null;
        [SerializeField] private Transform _rightAnchorPoint = null;
        
        [SerializeField] private List<Vector3> _patrolPath = new List<Vector3>();
        [SerializeField] private float _goNextPointDistance = 1f;
        //true direction = path++ false = path--;
        private bool _pathDirection = true;
        private int _pathIndex = 0;
        private NavMeshAgent _agent = null;
        
        private Vector3 _prevPosition = Vector3.zero;
        private bool _hacked = false;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.SetDestination(_patrolPath[_pathIndex]);
            GameEventManager.EnemyUpdate += NonHackedTick;

            GameEventManager.GamePauseStart += OnPauseEnter;
            GameEventManager.GamePauseExit += OnPauseExit;
        }

        // Update is called once per frame
        void NonHackedTick()
        {
            if (Vector3.SqrMagnitude(_patrolPath[_pathIndex] - transform.position) < _goNextPointDistance)
            {
                if (_pathDirection)
                {
                    if (_pathIndex < _patrolPath.Count - 1)
                    {
                        _pathIndex++;
                    }
                    else
                    {
                        _pathDirection = false;
                        _pathIndex--;
                    }
                }
                else
                {
                    if (_pathIndex > 0)
                    {
                        _pathIndex--;
                    }
                    else
                    {
                        _pathDirection = true;
                        _pathIndex++;
                    }
                }

                _agent.SetDestination(_patrolPath[_pathIndex]);
            }
        }

        public override void OnHackEnter()
        {
            _prevPosition = transform.position;
            GameEventManager.EnemyUpdate -= NonHackedTick;
            _agent.enabled = false;
            transform.position = _prevPosition;
            _hacked = true;
            CameraController.ChangeCamera(ObjectType.GroundEnemy, _cameraTransform);
            base.OnHackEnter();
        }

        public override void OnHackExit()
        {
            GameEventManager.EnemyUpdate += NonHackedTick;
            _agent.enabled = true;
            _agent.SetDestination(_patrolPath[_pathIndex]);
            _hacked = false;
            base.OnHackExit();
        }

        //this is so the pausing can set the object to be kinematic and non moving while keeping its velocity
        private Vector3 _prevVelocityRigid = Vector3.zero;
        private Vector3 _prevVelocityAgent = Vector3.zero;

        protected override void OnPauseEnter()
        {
            _rigidbody.isKinematic = true;
            _prevVelocityRigid = _rigidbody.velocity;
            _rigidbody.velocity = Vector3.zero;

            if (!_hacked)
            {
                _prevPosition = transform.position;
                _agent.enabled = false;
                transform.position = _prevPosition;

                _prevVelocityAgent = _agent.velocity;
                _agent.velocity = Vector3.zero;
                GameEventManager.EnemyUpdate -= NonHackedTick;
            }
            else
            {
                DisableInput();
            }
        }

        protected override void OnPauseExit()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _prevVelocityRigid;

            if (!_hacked)
            {
                _agent.enabled = true;
                GameEventManager.EnemyUpdate += NonHackedTick;
                _agent.velocity = _prevVelocityAgent;
                _agent.SetDestination(_patrolPath[_pathIndex]);
            }
            else
            {
                EnableInput();
            }

        }

        protected override void Tick()
        {
            //player version of movement
        }

        protected override void FixedTick()
        {
            //player version of movement
            Movement();
        }

        private void Movement()
        {
            if (_moveInput != Vector2.zero)
            {
                //managing spin of each rotation
                if (_moveInput.x > 0.1f)
                {
                    transform.RotateAround(_rightAnchorPoint.position, Vector3.up, _spinSpeed);
                }
                else if (_moveInput.x < -0.1f)
                {
                    transform.RotateAround(_leftAnchorPoint.position, Vector3.up, -_spinSpeed);
                }

                //managing forward movement as an absolute of forward direction
                if (Mathf.Abs(_moveInput.y) > 0.1f && _rigidbody.velocity.y >= 0)
                {
                    _rigidbody.velocity = transform.forward * (_moveSpeed * _moveInput.y);
                }
            }
            if (Mathf.Abs(_moveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(_rigidbody.velocity.x) > 0.2f || Mathf.Abs(_rigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = _rigidbody.velocity;
                    //takes off 10% of the current vel every physics update for "snappy" movement
                    newVel.z = newVel.z * 0.90f;
                    newVel.x = newVel.x * 0.90f;
                    _rigidbody.velocity = newVel;
                }
            }
        }

#if UNITY_EDITOR
        [SerializeField] private bool showPatrolPath = true;
        private void OnDrawGizmos()
        {
            if (!showPatrolPath)
                return;

            foreach (var VARIABLE in _patrolPath)
            {
                Gizmos.DrawCube(VARIABLE, Vector3.one);
            }
        }
        #endif

        protected override void MoveInputEnter(InputAction.CallbackContext a_context) =>
            _moveInput = a_context.ReadValue<Vector2>();

        protected override void MoveInputExit(InputAction.CallbackContext a_context) => _moveInput = Vector2.zero;

        protected override void CameraInputEnter(InputAction.CallbackContext a_context) =>
            _spinInput = a_context.ReadValue<Vector2>();

        protected override void CameraInputExit(InputAction.CallbackContext a_context) => _spinInput = Vector2.zero;

        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            _player.OnHackEnter();
            base.OnHackExit();
            OnHackExit();
        }

    }
}
