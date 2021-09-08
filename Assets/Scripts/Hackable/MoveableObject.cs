using Malicious.Player;
using Malicious.Core;
using Malicious.Interfaces;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Hackable
{
    public class MoveableObject : MonoBehaviour, IPlayerObject

    {
        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;

        [SerializeField] private Rigidbody _rigidbody = null;
        [SerializeField] private Transform _cameraOffset = null;
        [SerializeField] private Vector3 _rigOffset = Vector3.zero;
        

        //------Speed Variables----------------//
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _spinSpeed = 5f;

        //------Input Variables----------------//
        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _spinInput = Vector2.zero;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
        }

        public void OnHackEnter()
        {
            EnableInput();
            if (_rigidbody.isKinematic)
                _rigidbody.isKinematic = false;
            _nodeRenderer.material = _hackedMaterial;

        }

        public void OnHackExit()
        {
            DisableInput();
            if (_rigidbody.isKinematic == false)
                _rigidbody.isKinematic = true;

            _nodeRenderer.material = _defaultMaterial;
        }

        public void Tick()
        {
            //Logic
            SpinMovement();
        }

        public void FixedTick()
        {
            //Movement and etc
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


        private void SpinInputStart(InputAction.CallbackContext a_context)
        {
            _spinInput = a_context.ReadValue<Vector2>();
        }

        private void SpinInputEnd(InputAction.CallbackContext a_context)
        {
            _spinInput = Vector2.zero;
        }

        private void MoveInputStart(InputAction.CallbackContext a_context)
        {
            _moveInput = a_context.ReadValue<Vector2>();
        }

        private void MoveInputEnd(InputAction.CallbackContext a_context)
        {
            _moveInput = Vector2.zero;
        }

        public void OnHackValid()
        {
            _nodeRenderer.material = _hackValidMaterial;
        }

        public void OnHackFalse()
        {
            _nodeRenderer.material = _defaultMaterial;
        }

        public ObjectType ReturnType() => ObjectType.Moveable;

        public bool RequiresTruePlayerOffset() => true;

        public OffsetContainer GiveOffset()
        {
            OffsetContainer temp = new OffsetContainer();
            temp._offsetTransform = _cameraOffset;
            temp._rigOffset = _rigOffset;
            return temp;
        }

        public void SetOffset(Transform a_transform)
        {
            //This is so the camera offset is facing the same way as the player but keeping all other offset values
            float yValue = a_transform.rotation.eulerAngles.y;
            Vector3 prevEular = _cameraOffset.rotation.eulerAngles;
            _cameraOffset.rotation = Quaternion.Euler(prevEular.x, yValue, prevEular.z);
        }

        private void ExitObject(InputAction.CallbackContext a_context)
        {
            PlayerController.PlayerControl.ResetToPlayer();
        }

        private void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += MoveInputStart;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputEnd;
            GlobalData.InputManager.Player.Camera.performed += SpinInputStart;
            GlobalData.InputManager.Player.Camera.canceled += SpinInputEnd;
            GlobalData.InputManager.Player.Interaction.performed += ExitObject;
        }

        private void DisableInput()
        {
            GlobalData.InputManager.Player.Movement.performed -= MoveInputStart;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputEnd;
            GlobalData.InputManager.Player.Camera.performed -= SpinInputStart;
            GlobalData.InputManager.Player.Camera.canceled -= SpinInputEnd;
            GlobalData.InputManager.Player.Interaction.performed -= ExitObject;
        }


    }
}