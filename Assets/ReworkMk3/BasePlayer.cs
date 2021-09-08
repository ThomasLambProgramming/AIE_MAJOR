using System;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;
using Malicious.Core;

namespace Malicious.ReworkMk3
{
    public class BasePlayer : MonoBehaviour
    {
        //------Speed Variables----------------//
        [SerializeField] protected float _moveSpeed = 5f;
        [SerializeField] protected float _maxSpeed = 4f;
        [SerializeField] protected float _spinSpeed = 5f;
        //-------------------------------------//

        [SerializeField] protected Transform _cameraTransform = null;


        protected virtual void Tick()
        {
            
        }
        protected virtual void FixedTick()
        {
            
        }

        protected virtual void OnHackEnter()
        {
            
        }

        protected virtual void OnHackExit()
        {
            
        }
        protected virtual void OnHackValid()
        {
            
        }
        protected virtual void OnHackFalse()
        {
            
        }
        protected virtual Quaternion GiveRotation()
        {
            return transform.rotation;
        }

        protected virtual Quaternion GiveCameraRotation()
        {
            return _cameraTransform.rotation;
        }

        protected virtual void SetRotation(Quaternion a_rotation)
        {
            //Who knows what the fuck
        }

        protected virtual void SetCameraTransform(Transform a_cameraTransform)
        {
            _cameraTransform = a_cameraTransform;
        }

        protected virtual void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
            GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
            GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
            
        }
        protected virtual void DisableInput()
        {
            
        }

        protected virtual void MoveInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void MoveInputExit(InputAction.CallbackContext a_context){}
        protected virtual void JumpInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void JumpInputExit(InputAction.CallbackContext a_context){}
        protected virtual void CameraInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void CameraInputExit(InputAction.CallbackContext a_context){}
        protected virtual void InteractionInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void InteractionInputExit(InputAction.CallbackContext a_context){}
        protected virtual void DownInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void DownInputExit(InputAction.CallbackContext a_context){}
        protected virtual void PauseInputEnter(InputAction.CallbackContext a_context){}
        protected virtual void PauseInputExit(InputAction.CallbackContext a_context){}
    }
}