using System;
using UnityEngine;

namespace Malicious.Core
{
    /// <summary>
    /// This class is to avoid recreating MasterInput variables and etc
    /// </summary>
    public class GlobalData : MonoBehaviour
    {
        public static MasterInput InputManager;

        public static void MakeNewInput()
        {
            InputManager = new MasterInput();
        }

        public static void EnableInputMaster()
        {
            InputManager.Enable();
            InputManager.Player.Enable();
        }
    }
}