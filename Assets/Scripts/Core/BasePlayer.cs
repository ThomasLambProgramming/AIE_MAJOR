using System;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;
using Malicious.Core;

namespace Malicious.Core
{
    public class BasePlayer : MonoBehaviour
    {
        //Speed Variables//
        [SerializeField] protected float _moveSpeed = 5f;
        [SerializeField] protected float _maxSpeed = 4f;
        [SerializeField] protected float _spinSpeed = 5f;
        //-------------------------------------//

        //Component variables//
        protected Rigidbody _rigidbody = null;
        //-------------------------------------//

        //Camera Variables//
        [SerializeField] protected Transform _cameraTransform = null;
        //-------------------------------------//

        
        //I know this isnt good to have a player reference itself 
        //but its for every other hackable object to be able to exit out to the player
        //without needing any additional references and etc
        [HideInInspector] public Player _player = null;
        
        [SerializeField] protected bool _hasHoldOption = false;
        [SerializeField] public float _holdChargeTime = 2f;
        
        
        //Input Variables//
        protected Vector2 _moveInput = Vector2.zero;
        protected Vector2 _spinInput = Vector2.zero;
        //-------------------------------------//

        
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
        }

        public virtual void OnHackExit()
        {
            DisableInput();
            _moveInput = Vector2.zero;
            _spinInput = Vector2.zero;
            GameEventManager.PlayerUpdate -= Tick;
            GameEventManager.PlayerFixedUpdate -= FixedTick;
        }

        protected virtual void OnPauseEnter()
        {
        }

        protected virtual void OnPauseExit()
        {
        }

        public virtual Quaternion GiveRotation()
        {
            return transform.rotation;
        }

        public virtual Quaternion GiveCameraRotation()
        {
            return _cameraTransform.rotation;
        }

        public virtual void HoldOptionActivate()
        {
            
        }
        //By default this does nothing so that the player rotation
        //can be set without any specific isPlayer check
        public virtual void SetRotation(Quaternion a_rotation)
        {
        }

        public virtual void SetCameraTransform(Transform a_cameraTransform)
        {
            _cameraTransform = a_cameraTransform;
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

        protected virtual void PauseInputEnter(InputAction.CallbackContext a_context)
        {
        }
        protected virtual void PauseInputExit(InputAction.CallbackContext a_context)
        {
        }
    }
}