using System.Collections;
using Cinemachine;
using UnityEngine;

using Malicious.Core;
using Malicious.ReworkV2;
using UnityEditor;

namespace Malicious.Player
{
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController PlayerControl = null;

        //----------Camera Variables-----------------//
        public CinemachineVirtualCamera _mainCam = null;
        private Vector3 _currentRigOffset = Vector3.zero;
        private Vector3 _targetRigOffset = Vector3.zero;
        private Transform _currentOffset = null;
        //This is too store the original value of the transform to reset when moving
        [SerializeField] private Transform _previousOffset = null;
        private Transform _targetOffset = null;
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private float _camMoveSpeed = 10f;
        [SerializeField] private float _rigTransitionSpeed = 5f;
        private Cinemachine3rdPersonFollow _cinemachineSettings = null;

        private bool _rotating = false;
        //-------------------------------------------//
        
        
        //----------PLayer Variables-----------------//
        private IPlayerObject _currentPlayer = null;
        [SerializeField] private GameObject _truePlayerObject = null;
        private IPlayerObject _truePlayer = null;
        //-------------------------------------------//
        
        void Awake()
        {
            PlayerControl = this;
        }

        void Start()
        {
            _truePlayer = _truePlayerObject.GetComponent<IPlayerObject>();
            _currentPlayer = _truePlayer;
            _currentPlayer.OnHackEnter();
            _cinemachineSettings = _mainCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            
            OffsetContainer container = _currentPlayer.GiveOffset();
            _mainCam.Follow = container._offsetTransform;
            _mainCam.LookAt = container._offsetTransform;
            _cinemachineSettings.ShoulderOffset = container._rigOffset;
            
            
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
            if (_rotating)
                return;
            
            _currentPlayer.OnHackExit();

            Transform temp = _currentPlayer.GiveOffset()._offsetTransform;
            _previousOffset.position = temp.position;
            _previousOffset.rotation = temp.rotation;
            _mainCam.Follow = _previousOffset;
            _mainCam.LookAt = _previousOffset;
            
            _currentPlayer = a_interactable;
            
            //Only for moveable object but adding redundancy for future hackable objects
            if (_currentPlayer.RequiresOffset())
                _currentPlayer.SetOffset(_truePlayer.GiveOffset()._offsetTransform);
            
            _currentOffset = null;
            
            _currentPlayer.OnHackEnter();

            check = 0;
            _targetOffset = _currentPlayer.GiveOffset()._offsetTransform;
            _targetRigOffset = a_interactable.GiveOffset()._rigOffset;
            //Dont allow player input till after camera is finished moving
            GameEventManager.PlayerFixedUpdate -= FixedTick;
            GameEventManager.PlayerUpdate -= PlayerTick;
            
            StartCoroutine(TransitionCamera());
            
        }
        public void ResetToPlayer() => SwapPlayer(_truePlayer);

        Vector3 rigMoveAmount = Vector3.zero;
        private int check = 0;
        IEnumerator TransitionCamera()
        {
            bool donePosition = false;
            bool doneRotation = false;
            bool doneRig = false;
            
            rigMoveAmount = _targetRigOffset - _currentRigOffset;

            while (doneRotation == false || donePosition == false || doneRig == false)
            {
                check++;
                Vector3 moveAmount = _targetOffset.position - _previousOffset.position;
                if (moveAmount.sqrMagnitude > 1)
                {
                    _previousOffset.position += moveAmount.normalized * (_camMoveSpeed * Time.deltaTime);
                }
                else if (moveAmount.sqrMagnitude > 0.0001f)
                {
                    _previousOffset.position += moveAmount * (_camMoveSpeed * Time.deltaTime);
                }
                else
                    donePosition = true;


                
                if (Vector3.SqrMagnitude(_targetRigOffset - _currentRigOffset) > 0.01f)
                {
                    _currentRigOffset += rigMoveAmount * (_rigTransitionSpeed * Time.deltaTime);
                    _cinemachineSettings.ShoulderOffset = _currentRigOffset;
                }
                else
                    doneRig = true;
                
                
                if (_previousOffset.rotation != _targetOffset.rotation)
                    _previousOffset.rotation =
                        Quaternion.RotateTowards(
                            _previousOffset.rotation,
                            _targetOffset.rotation,
                            _rotationSpeed * Time.deltaTime);
                else
                    doneRotation = true;
                
                yield return null;
                /*
                 * Whatever other animations and etc can be played around now
                 */
            }
            GameEventManager.PlayerFixedUpdate += FixedTick;
            GameEventManager.PlayerUpdate += PlayerTick;
            _currentOffset = _targetOffset;
            _mainCam.Follow = _currentOffset;
            _mainCam.LookAt = _currentOffset;
            _rotating = false;
        }
    }
}