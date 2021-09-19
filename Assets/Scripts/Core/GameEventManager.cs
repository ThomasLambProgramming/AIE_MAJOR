using System;
using System.Collections;
using Malicious.UI;
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


        //When pause is activated all gameobjects that need to will stop movement and etc
        public static event Action GamePauseStart;
        public static event Action GamePauseExit;
        
        public static event Action PlayerHit;
        public static event Action PlayerHealed;
        public static event Action PlayerDead;

        //I really wanted to avoid using this but I couldnt call a coroutine from a static function
        //so for fade transitions it would be worse to try and make some sort of wait then just use a singleton
        public static GameEventManager _CurrentManager = null; 

        private static bool _paused = false;
        
        private static int _playerHealth = 3;
        
        //Getter for the player health so it can be seen without chance of changing the value outside of 
        //the class
        public static int CurrentHealth() => _playerHealth;

        [SerializeField] private FadeTransition _fadeTransitionInit = null;
        private static FadeTransition _fadeTransition = null;
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
            _CurrentManager = this;
            _fadeTransition = _fadeTransitionInit;
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

        //Used by ui button (doesnt like the input function)
        public static void ResumePlay()
        {
            if (_paused)
            {
                GamePauseExit?.Invoke();
                _paused = false;
            }
        }

        /// <summary>
        /// Manage player health in the game event manager so
        /// all other objects can get updated without knowing the players current health
        /// </summary>
        public static void PlayerHitFunc()
        {
            if (_playerHealth >= 1)
                PlayerHit?.Invoke();
            
            _playerHealth -= 1;
            
            if (_playerHealth <= 0)
            {
                //fade to black
                _fadeTransition.FadeOut();
                _CurrentManager.WaitForFade();
            }
            
        }

        private void WaitForFade()
        {
            StartCoroutine(FadeWait());
        }

        private IEnumerator FadeWait()
        {
            yield return new WaitForSeconds(1f);
            _playerHealth = 3;
            PlayerDead?.Invoke();
            yield return new WaitForSeconds(1f);
            _fadeTransition.FadeIn();
        }

        public static void PlayerHealedFunc()
        {
            if (_playerHealth < 3)
            {
                _playerHealth++;
                PlayerHealed?.Invoke();
            }
        }
    }
}
