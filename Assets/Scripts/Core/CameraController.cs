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

        //named init to not accidently use in functions
        [SerializeField] private CinemachineVirtualCamera starting_Camera = null;
        [SerializeField] private CinemachineVirtualCamera init_player = null;
        [SerializeField] private CinemachineVirtualCamera init_moveable = null;
        [SerializeField] private CinemachineVirtualCamera init_pointOfInterest = null;
        [SerializeField] private CinemachineVirtualCamera init_wire = null;
        [SerializeField] private CinemachineVirtualCamera init_groundEnemy = null;
        [SerializeField] private CinemachineVirtualCamera init_flyingEnemy = null;
        
        private static CinemachineVirtualCamera _player = null;
        private static CinemachineVirtualCamera _moveable = null;
        private static CinemachineVirtualCamera _pointOfInterest = null;
        private static CinemachineVirtualCamera _wire = null;
        private static CinemachineVirtualCamera _groundEnemy = null;
        private static CinemachineVirtualCamera _flyingEnemy = null;
        private static CinemachineVirtualCamera _currentCamera = null;

        [SerializeField] private CinemachineBrain init_BrainCamera = null;
        public static CinemachineBrain _cameraBrain = null;
        private void Start()
        {
            _resetPrio = init_resetPrio;
            _activePrio = init_activePrio;
            
            _currentCamera = starting_Camera;
            _currentCamera.Priority = _activePrio;
            
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
            _currentCamera.Priority = _resetPrio;

            switch (a_type)
            {
                case ObjectType.Player:
                    _currentCamera = _player;
                    break;
                case ObjectType.Moveable:
                    _currentCamera = _moveable;
                    break;
                case ObjectType.PointOfInterest:
                    _currentCamera = _pointOfInterest;
                    break;
                case ObjectType.Wire:
                    _currentCamera = _wire;
                    break;
                case ObjectType.GroundEnemy:
                    _currentCamera = _groundEnemy;
                    break;
                case ObjectType.FlyingEnemy:
                    _currentCamera = _flyingEnemy;
                    break;
            }

            //This prio is for which camera is currently the active one
            if (a_requireOffset)
            {
                _currentCamera.LookAt = a_offset;
                _currentCamera.Follow = a_offset;
            }
            _currentCamera.Priority = _activePrio;
        }
    }
}