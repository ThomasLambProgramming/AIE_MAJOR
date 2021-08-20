using Cinemachine;
using UnityEngine;

using Malicious.Core;
using Malicious.ReworkV2;

namespace Malicious.Player
{
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController PlayerControl = null;

        public CinemachineVirtualCamera _mainCam = null;
        
        private Transform _currentOffset = null;
        //This is too store the original value of the transform to reset when moving
        private Transform _previousOffset = null;
        private Transform _targetOffset = null;

        private IPlayerObject _currentPlayer = null;

        [SerializeField] private GameObject _truePlayerObject = null;
        private IPlayerObject _truePlayer = null;
        
        void Awake()
        {
            PlayerControl = this;
        }

        void Start()
        {
            _truePlayer = _truePlayerObject.GetComponent<IPlayerObject>();
            _currentPlayer = _truePlayer;
            _currentPlayer.OnHackEnter();
            _mainCam.Follow = _currentPlayer.GiveOffset();
            
            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;
        }
        private void PlayerTick()
        {
            //Update Current Player
            _currentPlayer.Tick();
        }
        private void FixedTick()
        {
            //Update Current Player
            _currentPlayer.FixedTick();
        }
        
        public void SwapPlayer(IPlayerObject a_interactable)
        {
            _currentPlayer.OnHackExit();
            _currentPlayer = a_interactable;
            _currentPlayer.OnHackEnter();

            _mainCam.Follow = _currentPlayer.GiveOffset();
            /*
             * Run camera routine to allow for camera movement and what not
             * +animations
             * As it will be a coroutine reset to player will use it as well for moving camera to correct
             * position
             */
            
        }

        public void ResetToPlayer()
        {
            _currentPlayer.OnHackExit();
            _currentPlayer = _truePlayer;
            _currentPlayer.OnHackEnter();
            _mainCam.Follow = _currentPlayer.GiveOffset();
        }
        
    }
}