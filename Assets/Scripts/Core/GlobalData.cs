using UnityEngine;

namespace Malicious.Core
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