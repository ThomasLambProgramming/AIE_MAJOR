using System;
using UnityEngine;

namespace Malicious.ReworkV2
{
    /// <summary>
    /// This class is to avoid recreating MasterInput variables and etc
    /// </summary>
    public class GlobalData : MonoBehaviour
    {
        public static MasterInput InputManager;
        private void Awake()
        {
            InputManager = new MasterInput();
        }
    }
}