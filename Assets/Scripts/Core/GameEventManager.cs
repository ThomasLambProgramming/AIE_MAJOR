using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Core
{
    public class GameEventManager : MonoBehaviour
    {
        //These events is seperated to allow for more options
        //such as disabling the player update while a cutscene is active
        //without needing heaps of if statements on when the player can and 
        //cannot move
    
        /// <summary>
        /// Player update event
        /// </summary>
        public static event Action PlayerUpdate;
        //fixed update is needed as well for physics updates
        /// <summary>
        /// Player Fixed update event
        /// </summary>
        public static event Action PlayerFixedUpdate;
    
        /// <summary>
        /// Update event for all enemies
        /// </summary>
        public static event Action EnemyUpdate;
        /// <summary>
        /// Fixed Update event for all enemies
        /// </summary>
        public static event Action EnemyFixedUpdate;
    
        /// <summary>
        /// General update is for all objects needing an update loop
        /// outside of enemies and the player
        /// </summary>
        public static event Action GeneralUpdate;
        /// <summary>
        /// General update is for all objects needing an update loop
        /// outside of enemies and the player
        /// </summary>
        public static event Action GeneralFixedUpdate;


        private static bool _paused = false;
        //When pause is activated all gameobjects that need to will stop movement and etc
        public static event Action GamePauseStart;
        public static event Action GamePauseExit;

        //This is to avoid references for the designers 
        public static event Action UiHealthDown;
        public static event Action UiHealthUp;

        void Update()
        {
            PlayerUpdate?.Invoke();
            EnemyUpdate?.Invoke();
            GeneralUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            PlayerFixedUpdate?.Invoke();
            EnemyFixedUpdate?.Invoke();
            GeneralFixedUpdate?.Invoke();
        }

        private void Start()
        {
            GlobalData.InputManager.Enable();
            GlobalData.InputManager.Player.Enable();
            GlobalData.InputManager.Player.Pause.performed += PausePressed;
        }

        private void PausePressed(InputAction.CallbackContext a_context)
        {
            if (!_paused)
            {
                GamePauseStart?.Invoke();
                _paused = true;
            }
            else
            {
                GamePauseExit?.Invoke();
                _paused = false;
            }
        }

        public static void ResumePlay()
        {
            if (_paused)
            {
                GamePauseExit?.Invoke();
                _paused = false;
            }
        }
        

        /// <summary>
        /// THIS IS ONLY TO BE USED WHEN ABSOLUTELY NEEDED
        /// MAINLY FOR DEBUG
        /// </summary>
        public void ClearAllEvents()
        {
            PlayerUpdate        = null;
            PlayerFixedUpdate   = null;
            EnemyUpdate         = null;
            EnemyFixedUpdate    = null;
            GeneralUpdate       = null;
            GeneralFixedUpdate  = null;
        }
    }
}
