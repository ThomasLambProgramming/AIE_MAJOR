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
        [SerializeField] private CinemachineFreeLook init_player = null;
        [SerializeField] private CinemachineVirtualCamera init_moveable = null;
        [SerializeField] private CinemachineVirtualCamera init_pointOfInterest = null;
        [SerializeField] private CinemachineVirtualCamera init_wire = null;
        [SerializeField] private CinemachineVirtualCamera init_groundEnemy = null;
        [SerializeField] private CinemachineVirtualCamera init_flyingEnemy = null;
        
        private static CinemachineFreeLook _player = null;
        private static CinemachineVirtualCamera _moveable = null;
        private static CinemachineVirtualCamera _pointOfInterest = null;
        private static CinemachineVirtualCamera _wire = null;
        private static CinemachineVirtualCamera _groundEnemy = null;
        private static CinemachineVirtualCamera _flyingEnemy = null;
        private static CinemachineVirtualCamera _currentHackableCamera = null;

        [SerializeField] private CinemachineBrain init_BrainCamera = null;
        public static CinemachineBrain _cameraBrain = null;
        private void Start()
        {
            _resetPrio = init_resetPrio;
            _activePrio = init_activePrio;
            
            _player = init_player;
            _moveable = init_moveable;
            _pointOfInterest = init_pointOfInterest;
            _wire = init_wire;
            _groundEnemy = init_groundEnemy;
            _flyingEnemy = init_flyingEnemy;

            _cameraBrain = init_BrainCamera;
        }


        public static void ChangeCamera(
            ObjectType a_type, 
            bool a_requireOffset = false, 
            Transform a_offset = null)
        {
            //we know its the current if the prio is above the reset amount
            if (_player.Priority > _resetPrio)
            {
                _player.Priority = 0;
            }

            float newRotationY = 0f;
            
            if (_currentHackableCamera != null)
            {
                _currentHackableCamera.Priority = _resetPrio;
                newRotationY = _currentHackableCamera.transform.rotation.eulerAngles.y;
            }

            
            switch (a_type)
            {
                case ObjectType.Player:
                    //The player has to be different as it is not the same type
                    _player.Priority = 20;
                    break;
                case ObjectType.Moveable:
                    _currentHackableCamera = _moveable;
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

            if (_player.Priority > 0)
            {
                Vector3 prevRotation = _player.transform.rotation.eulerAngles;
                if (newRotationY != 0)
                    _player.transform.rotation = Quaternion.Euler(prevRotation.x, newRotationY, prevRotation.z);
                //Lookat and follow + prio doesnt need to be changed if its the player
                return;
            }
            
            if (a_requireOffset)
            {
                _currentHackableCamera.LookAt = a_offset;
                _currentHackableCamera.Follow = a_offset;
            }
            _currentHackableCamera.Priority = _activePrio;
        }
    }
}