using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Interfaces;
using UnityEngine;
using Malicious.Core;
using Malicious.Player;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Malicious
{
    [SelectionBase]
    public class GroundEnemy : MonoBehaviour, IPlayerObject
    {
        [SerializeField] private float _moveSpeed = 100f;
        [SerializeField] private float _turningSpeed = 10f;

        [SerializeField] private List<Vector3> _PatrolPath = new List<Vector3>();
        private int _pathIndex = 0;
        [SerializeField] private float _goNextPointDistance = 1f;
        //true direction = path++ false = path--;
        private bool _pathDirection = true;
        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;

        [SerializeField] private Transform _cameraOffset = null;
        [SerializeField] private Vector3 _rigOffset = Vector3.zero;

        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _spinInput = Vector2.zero;
        private Rigidbody _rigidbody = null;
        
        private NavMeshAgent _agent = null;
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
            _agent.SetDestination(_PatrolPath[_pathIndex]);
            GameEventManager.EnemyUpdate += NonHackedTick;
        }

        // Update is called once per frame
        void NonHackedTick()
        {
            if (Vector3.SqrMagnitude(_PatrolPath[_pathIndex] - transform.position) < _goNextPointDistance)
            {
                if (_pathDirection)
                {
                    if (_pathIndex < _PatrolPath.Count - 1)
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
                _agent.SetDestination(_PatrolPath[_pathIndex]);
            }
        }
#if UNITY_EDITOR
        [SerializeField] private bool showPatrolPath = true;
        private void OnDrawGizmos()
        {
            if (!showPatrolPath)
                return;
            
            foreach (var VARIABLE in _PatrolPath)
            {
                Gizmos.DrawCube(VARIABLE, Vector3.one);
            }
        }
#endif
        public void OnHackEnter()
        {
            GameEventManager.EnemyUpdate -= NonHackedTick;
            _agent.enabled = false;
            _nodeRenderer.material = _hackedMaterial;
            EnableInput();
        }

        public void OnHackExit()
        {
            GameEventManager.EnemyUpdate += NonHackedTick;
            _agent.enabled = true;
            _agent.SetDestination(_PatrolPath[_pathIndex]);
            _nodeRenderer.material = _defaultMaterial;
            DisableInput();
        }

        public void Tick()
        {
            //player version of movement
        }

        public void FixedTick()
        {
            //player version of movement
            Movement();
            SpinMovement();
        }

        private void Movement()
        {
            if (_moveInput != Vector2.zero)
            {
                float currentYAmount = _rigidbody.velocity.y;
                Vector3 newVel =
                    transform.forward * (_moveInput.y * _moveSpeed * Time.deltaTime) +
                    transform.right * (_moveInput.x * _moveSpeed * Time.deltaTime);
                newVel.y = currentYAmount;
                _rigidbody.velocity = newVel;
            }

            if (Mathf.Abs(_moveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(_rigidbody.velocity.x) > 0.2f || Mathf.Abs(_rigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = _rigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * 0.95f;
                    newVel.x = newVel.x * 0.95f;
                    _rigidbody.velocity = newVel;
                }
            }
        }

        private void SpinMovement()
        {
            if (_spinInput != Vector2.zero)
            {
                transform.Rotate(new Vector3(0, _spinInput.x * _turningSpeed * Time.deltaTime, 0));
            }
        }

        public OffsetContainer GiveOffset()
        {
            OffsetContainer temp = new OffsetContainer();
            temp._offsetTransform = _cameraOffset;
            temp._rigOffset = _rigOffset;
            return temp;
        }

        public bool RequiresTruePlayerOffset() => false;
        

        public void SetOffset(Transform a_offset)
        {
            _cameraOffset = a_offset;
        }

        public void OnHackValid()
        {
            _nodeRenderer.material = _hackValidMaterial;
        }

        public void OnHackFalse()
        {
            _nodeRenderer.material = _defaultMaterial;
        }

        private void MoveInputEnter(InputAction.CallbackContext a_context) => _moveInput = a_context.ReadValue<Vector2>();
        private void MoveInputExit(InputAction.CallbackContext a_context) => _moveInput = Vector2.zero;
        private void SpinInputEnter(InputAction.CallbackContext a_context) => _spinInput = a_context.ReadValue<Vector2>();
        private void SpinInputExit(InputAction.CallbackContext a_context) => _spinInput = Vector2.zero;
        
        private void Interact(InputAction.CallbackContext a_context)
        {
            PlayerController.PlayerControl.ResetToPlayer(transform.position, transform.rotation);
        }

        private void EnableInput()
        {
            GlobalData.InputManager.Player.Enable();
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Camera.performed += SpinInputEnter;
            GlobalData.InputManager.Player.Camera.canceled += SpinInputExit;
            GlobalData.InputManager.Player.Interaction.performed += Interact;
        }

        private void DisableInput()
        {
            GlobalData.InputManager.Player.Disable();
            GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
            GlobalData.InputManager.Player.Camera.performed -= SpinInputEnter;
            GlobalData.InputManager.Player.Camera.canceled -= SpinInputExit;
            GlobalData.InputManager.Player.Interaction.performed -= Interact;
        }
    }
}
