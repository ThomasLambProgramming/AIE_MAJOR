using System;
using System.Collections;
using Malicious.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Core
{
    public class GameEventManager : MonoBehaviour
    {
        public AudioSource _hackingInAudio = null;
        public AudioSource _hackingOutAudio = null;
        public AudioSource _heartAudio = null;
        public AudioSource _pauseAudio = null;


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

        public static event Action PlayerStopInput;
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
        //I cant be bothered at this point and im just making things static so its easy to reference
        public static FadeTransition _fadeTransition = null;

        
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

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _CurrentManager = this;
            GlobalData.MakeNewInput();
            GlobalData.EnableInputMaster();
            GlobalData.InputManager.Player.Pause.performed += PausePressed;
            _fadeTransition = _fadeTransitionInit;
            _fadeTransition.FadeIn();
           
        }

        private void PausePressed(InputAction.CallbackContext a_context)
        {
            if (VoiceText._voiceText != null && VoiceText._voiceText._displaying)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                VoiceText._voiceText.HideText();
                return;
            }

            if (!_paused)
            {
                _pauseAudio.Play();
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
                GamePauseStart?.Invoke();
                _paused = true;
            }
            else
            {
                GlobalData.SaveSettings();
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
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
                Time.timeScale = 1f;
                _paused = false;
                GlobalData.SaveSettings();
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
                //Player shader for death play 
                _fadeTransition.FadeOut();
                PlayerStopInput?.Invoke();
                _CurrentManager.WaitForFade();
            }
            
        }

        public static void PlayerResetAlphaTest()
        {
            _playerHealth = 0;
            PlayerHitFunc();
            ResumePlay();
        }
        private void WaitForFade()
        {
            StartCoroutine(FadeWait());
        }

        private IEnumerator FadeWait()
        {
            yield return new WaitForSeconds(1.3f);
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

        public static void SpikeHit()
        {
            _playerHealth = 0;
            PlayerHitFunc();
        }

        public static void Reset()
        {
            if (GlobalData.InputManager != null)
                GlobalData.InputManager.Dispose();

            PlayerUpdate        = null;
            PlayerFixedUpdate   = null;
            EnemyUpdate         = null;
            EnemyFixedUpdate    = null;
            GeneralUpdate       = null;
            GeneralFixedUpdate  = null;
            GamePauseStart      = null;
            GamePauseExit       = null;
            PlayerHit           = null;
            PlayerHealed        = null;
            PlayerDead          = null;



            Time.timeScale = 1f;
        }
    }
}
