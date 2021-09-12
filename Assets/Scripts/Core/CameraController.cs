using UnityEngine;
using Cinemachine;

namespace Malicious.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private int init_resetPrio = 0;
        [SerializeField] private int init_activePrio = 10;
        
        private static int _resetPrio = 0;
        private static int _activePrio = 10;

        //named init to not accidentally use in functions
        [SerializeField] private Transform init_mainCamTransform = null;
        [SerializeField] private CinemachineFreeLook init_player = null;
        [SerializeField] private CinemachineVirtualCamera init_moveable = null;
        [SerializeField] private CinemachineVirtualCamera init_pointOfInterest = null;
        [SerializeField] private CinemachineVirtualCamera init_wire = null;
        [SerializeField] private CinemachineVirtualCamera init_groundEnemy = null;
        [SerializeField] private CinemachineVirtualCamera init_flyingEnemy = null;

        private static Transform _mainCamTransform = null;
        private static CinemachineFreeLook _player = null;
        private static CinemachineVirtualCamera _moveable = null;
        private static CinemachineVirtualCamera _pointOfInterest = null;
        private static CinemachineVirtualCamera _wire = null;
        private static CinemachineVirtualCamera _groundEnemy = null;
        private static CinemachineVirtualCamera _flyingEnemy = null;
        private static CinemachineVirtualCamera _currentHackableCamera = null;
        
        private void Start()
        {
            _mainCamTransform = init_mainCamTransform;
            
            _resetPrio = init_resetPrio;
            _activePrio = init_activePrio;
            
            _player = init_player;
            _moveable = init_moveable;
            _pointOfInterest = init_pointOfInterest;
            _wire = init_wire;
            _groundEnemy = init_groundEnemy;
            _flyingEnemy = init_flyingEnemy;
        }


        public static void ChangeCamera(
            ObjectType a_type,
            Transform a_offset = null)
        {
            //we know its the current if the prio is above the reset amount
            if (_player.Priority > _resetPrio)
            {
                _player.Priority = 0;
            }
            
            
            if (_currentHackableCamera != null)
            {
                _currentHackableCamera.Priority = _resetPrio;
            }

            
            switch (a_type)
            {
                case ObjectType.Player:
                    //The player has to be different as it is not the same type
                    _player.Priority = 20;
                    Vector3 currentPlayerRot = _player.transform.rotation.eulerAngles;
                    currentPlayerRot.y = _currentHackableCamera.transform.rotation.eulerAngles.y;
                    _player.transform.rotation = Quaternion.Euler(currentPlayerRot);
                    return;
                case ObjectType.Moveable:
                    _currentHackableCamera = _moveable;
                    float newY = _mainCamTransform.rotation.eulerAngles.y;
                    Vector3 offsetEular = a_offset.rotation.eulerAngles;
                    offsetEular.y = newY;
                    a_offset.rotation = Quaternion.Euler(offsetEular);
                    break;
                case ObjectType.PointOfInterest:
                    _currentHackableCamera = _pointOfInterest;
                    break;
                case ObjectType.Wire:
                    _currentHackableCamera = _wire;
                    break;
                case ObjectType.GroundEnemy:
                    _currentHackableCamera = _groundEnemy;
                    break;
                case ObjectType.FlyingEnemy:
                    _currentHackableCamera = _flyingEnemy;
                    break;
            }
            
            
            if (a_offset != null)
            {
                _currentHackableCamera.LookAt = a_offset;
                _currentHackableCamera.Follow = a_offset;
            }
            _currentHackableCamera.Priority = _activePrio;
        }
    }
}