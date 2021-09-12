using System;
using Malicious.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Malicious.Hackable
{
    [RequireComponent(typeof(HackableField))]
    public class MoveableBlock : BasePlayer
    {
        [SerializeField] private Transform _cameraOffset = null;

        [SerializeField] private bool _faceBoxOnExit = true;
        [SerializeField] private Transform _exitPosition = null;
        [SerializeField] private Collider _exitBox = null;
        [SerializeField] private Vector3 _exitDirection = Vector3.zero;
        [SerializeField] private float _exitForce = 4f;
        
        [SerializeField] private UnityEvent _onHackEnterEvent = null;
        [SerializeField] private UnityEvent _onHackExitEvent = null;
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _cameraTransform = _cameraOffset;
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
                //camForward.y = 0;

                Vector3 camRight = _cameraOffset.right;
                //camRight.y = 0;

                float currentYAmount = _rigidbody.velocity.y;
                Vector3 newVel =
                    camForward * (_moveInput.y * _moveSpeed * Time.deltaTime) +
                    camRight * (_moveInput.x * _moveSpeed * Time.deltaTime);
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
                _cameraOffset.RotateAround(transform.position, Vector3.up,
                    _spinInput.x * _spinSpeed * Time.deltaTime);
            }
        }

        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            //exit out
            _player.transform.position = _exitPosition.position;
            Vector3 rotationDirection = Vector3.zero;
            
            if (_faceBoxOnExit) 
                rotationDirection = (transform.position - _exitPosition.position).normalized;
            else
                rotationDirection = (_exitPosition.position - transform.position).normalized;
            
            _player.transform.rotation = Quaternion.LookRotation(rotationDirection);
            _player.OnHackEnter();
            _player.LaunchPlayer(_exitDirection * _exitForce);
            OnHackExit();
        }

        public override void OnHackEnter()
        {
            _onHackEnterEvent?.Invoke();
            if (_rigidbody.isKinematic)
                _rigidbody.isKinematic = false;
            _exitBox.enabled = true;
            base.OnHackEnter();
            
            CameraController.ChangeCamera(ObjectType.Moveable, _cameraOffset);
            //Check the materials as well for hack indication
        }
        public override void OnHackExit()
        {
            _onHackExitEvent?.Invoke();
            if (_rigidbody.isKinematic == false)
                _rigidbody.isKinematic = true;
            base.OnHackExit();
            _exitBox.enabled = false;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_exitPosition != null)
                Gizmos.DrawLine(_exitPosition.position, (_exitPosition.position + _exitDirection * 4f));
            //draw exit velocities and etc
        }
#endif
    }
}