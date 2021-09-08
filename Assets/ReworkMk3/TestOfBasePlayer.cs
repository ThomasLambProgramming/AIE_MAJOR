using System;
using Malicious.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.ReworkMk3
{
    public class TestOfBasePlayer : BasePlayer
    {
        public void Start()
        {
            GlobalData.InputManager.Enable();
            GlobalData.InputManager.Player.Enable();
            EnableInput();
        }

        protected override void MoveInputEnter(InputAction.CallbackContext a_context)
        {
            Debug.Log("TESSSTTTT");
        }
    }
}