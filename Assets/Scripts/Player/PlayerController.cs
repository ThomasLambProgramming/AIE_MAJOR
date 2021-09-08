using System.Collections;
using Cinemachine;
using UnityEngine;

using Malicious.Core;
using Malicious.Interfaces;
using Malicious.UI;

namespace Malicious.Player
{
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController PlayerControl = null;

        
        //-----------Wire Variables------------------//
        //This is to have one globally used wire model to give to any wire asset
        //and not have to link it 
        public GameObject _wireModel = null;
        public Transform _wireModelOffset = null;
        //-------------------------------------------//
        
        
        //----------PLayer Variables-----------------//
        [SerializeField] private GameObject _playerHudObject = null;
        private PlayerHud _playerHud = null;
        
        [SerializeField] private int _playerHealth = 3;
        
        private IPlayerObject _currentPlayer = null;
        [SerializeField] private GameObject _truePlayerObject = null;

        private IPlayerObject _truePlayer = null;
        //-------------------------------------------//

        private bool _paused = false;
        private void PlayerTick()
        {
            if (_paused)
                return;
            //Update Current Player
            _currentPlayer.Tick();
        }

        private void FixedTick()
        {
            if (_paused)
                return;
            //Update Current Player
            _currentPlayer.FixedTick();
        }
        
        void Awake()
        {
            PlayerControl = this;
        }

        void Start()
        {
            _truePlayer = _truePlayerObject.GetComponent<IPlayerObject>();
            _currentPlayer = _truePlayer;
            _currentPlayer.OnHackEnter();
            
            _playerHud = _playerHudObject.GetComponent<PlayerHud>();

            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;

            GameEventManager.GamePauseStart += PauseEnter;
            GameEventManager.GamePauseExit += PauseExit;
        }

        private void PauseEnter() => _paused = true;
        private void PauseExit() => _paused = false;

        public void PlayerDead()
        {
            
        }

        public void PlayerHit()
        {
            _playerHud.RemoveHealth();
            if (_playerHealth <= 0)
            {
                PlayerDead();
            }
        }

        //the amount has been added to allow for the use of multi healing in case that option is added later
        public void PlayerHealed(int a_amount)
        {
            for (int i = 0; i < a_amount; i++)
            {
                _playerHud.AddHealth();
            }
        }

        public void SwapPlayer(IPlayerObject a_interactable)
        {
            _currentPlayer.OnHackExit();
            
            _currentPlayer = a_interactable;
            _currentPlayer.OnHackEnter();
            
            
            CameraController.ChangeCamera(
                _currentPlayer.ReturnType(),
                true,
                _currentPlayer.GiveOffset()._offsetTransform);
            
        }
        
        public void ResetToPlayer()
        {
            _truePlayerObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            SwapPlayer(_truePlayer);
        }
    }
}