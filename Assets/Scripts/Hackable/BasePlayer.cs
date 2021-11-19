using UnityEngine.InputSystem;
using UnityEngine;
using Malicious.Core;
using Unity.Mathematics;

namespace Malicious.Hackable
{
    public class BasePlayer : MonoBehaviour
    {
        //Speed Variables//
        [SerializeField] protected float _moveSpeed = 5f;
        [SerializeField] protected float _maxSpeed = 4f;
        [SerializeField] protected float _spinSpeed = 5f;
        //-------------------------------------//

        //Camera Variables//
        [SerializeField] protected Transform _cameraTransform = null;
        //-------------------------------------//

        //Reference to the player that hacked into it
        [HideInInspector] public Player _player = null;
        
        //HackableRigidbody
        protected Rigidbody _rigidbody = null;
        
        [SerializeField] public bool _hasHoldOption = false;
        [SerializeField] public float _holdChargeTime = 2f;

        //Input Variables//
        protected Vector2 _moveInput = Vector2.zero;
        protected Vector2 _spinInput = Vector2.zero;
        //-------------------------------------//
        private HackableField _hackField = null;

        public virtual void Start()
        {
            _hackField = GetComponent<HackableField>();
        }
        protected virtual void Tick()
        {
        }
        protected virtual void FixedTick()
        {
        }
        public virtual void OnHackEnter()
        {
            EnableInput();
            _moveInput = Vector2.zero;
            _spinInput = Vector2.zero;
            GameEventManager.PlayerUpdate += Tick;
            GameEventManager.PlayerFixedUpdate += FixedTick;
            Vector3 eularRot = transform.rotation.eulerAngles;
            eularRot.x = 0;
            transform.rotation = Quaternion.Euler(eularRot);
        }
        public virtual void OnHackExit()
        {
            DisableInput();

            if (_player == null)
            {
                _player = Player._player;
            }
            _moveInput = Vector2.zero;
            _spinInput = Vector2.zero;
            GameEventManager.PlayerUpdate -= Tick;
            GameEventManager.PlayerFixedUpdate -= FixedTick;
            if (_hackField != null)
            {
                _hackField.ResetColors();
            }
        }
        public virtual void HoldOptionActivate()
        {
            
        }

        protected bool _inFanUp = false;
        protected bool _inFanHoriz = false;
        protected int _amountOfUpFans = 0;
        protected int _amountOfHozFans = 0;

        public void EnteredFan(bool a_isUp)
        {
            if (a_isUp)
            {
                _inFanUp = true;
                _amountOfUpFans++;
            }
            else
            {
                //_moveSpeed = _moveSpeed * 0.5f;
                _amountOfHozFans++;
                _inFanHoriz = true;
            }
        }

        public virtual void ExitedFan(bool a_isUp)
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
                    //_moveSpeed *= 2;
                    _amountOfHozFans = 0;
                    _inFanHoriz = false;
                }
            }
        }

        protected virtual void OnPauseEnter()
        {
        }
        protected virtual void OnPauseExit()
        {
        }

        #region Input
        
        protected virtual void MoveInputEnter(InputAction.CallbackContext a_context)
        {
            _moveInput = a_context.ReadValue<Vector2>();
        }

        protected virtual void MoveInputExit(InputAction.CallbackContext a_context)
        {
            _moveInput = Vector2.zero;
        }

        protected virtual void JumpInputEnter(InputAction.CallbackContext a_context)
        {
        }

        protected virtual void JumpInputExit(InputAction.CallbackContext a_context)
        {
        }

        protected virtual void CameraInputEnter(InputAction.CallbackContext a_context)
        {
            _spinInput = a_context.ReadValue<Vector2>();
        }

        protected virtual void CameraInputExit(InputAction.CallbackContext a_context)
        {
            _spinInput = Vector2.zero;
        }

        protected virtual void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
        }

        protected virtual void InteractionInputExit(InputAction.CallbackContext a_context)
        {
        }

        protected virtual void DownInputEnter(InputAction.CallbackContext a_context)
        {
        }

        protected virtual void DownInputExit(InputAction.CallbackContext a_context)
        {
        }
        protected virtual void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled += JumpInputExit;
            GlobalData.InputManager.Player.Camera.performed += CameraInputEnter;
            GlobalData.InputManager.Player.Camera.canceled += CameraInputExit;
            GlobalData.InputManager.Player.Interaction.performed += InteractionInputEnter;
            GlobalData.InputManager.Player.Interaction.canceled += InteractionInputExit;
            GlobalData.InputManager.Player.Down.performed += DownInputEnter;
            GlobalData.InputManager.Player.Down.canceled += DownInputExit;
        }

        protected virtual void DisableInput()
        {
            GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed -= JumpInputEnter;
            GlobalData.InputManager.Player.Jump.canceled -= JumpInputExit;
            GlobalData.InputManager.Player.Camera.performed -= CameraInputEnter;
            GlobalData.InputManager.Player.Camera.canceled -= CameraInputExit;
            GlobalData.InputManager.Player.Interaction.performed -= InteractionInputEnter;
            GlobalData.InputManager.Player.Interaction.canceled -= InteractionInputExit;
            GlobalData.InputManager.Player.Down.performed -= DownInputEnter;
            GlobalData.InputManager.Player.Down.canceled -= DownInputExit;
        }
        #endregion
    }
}