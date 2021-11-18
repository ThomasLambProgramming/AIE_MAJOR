using UnityEngine;
using Cinemachine;

namespace Malicious.Core
{
    public class CameraController : MonoBehaviour
    {
        //named init to not accidentally use in functions
        [SerializeField] private Transform init_mainCamTransform = null;
        [SerializeField] private CinemachineVirtualCamera init_player = null;
        [SerializeField] private CinemachineVirtualCamera init_moveable = null;
        [SerializeField] private CinemachineVirtualCamera init_spring = null;
        [SerializeField] private CinemachineVirtualCamera init_pointOfInterest = null;
        [SerializeField] private CinemachineVirtualCamera init_wire = null;
        [SerializeField] private CinemachineVirtualCamera init_groundEnemy = null;
        [SerializeField] private CinemachineVirtualCamera init_flyingEnemy = null;

        private static Transform _mainCamTransform = null;
        private static CinemachineVirtualCamera _player = null;
        private static CinemachineVirtualCamera _moveable = null;
        private static CinemachineVirtualCamera _spring = null;
        private static CinemachineVirtualCamera _pointOfInterest = null;
        private static CinemachineVirtualCamera _wire = null;
        private static CinemachineVirtualCamera _groundEnemy = null;
        private static CinemachineVirtualCamera _flyingEnemy = null;
        private static CinemachineVirtualCamera _currentHackableCamera = null;
        
        [SerializeField] private int init_resetPrio = 0;
        [SerializeField] private int init_activePrio = 10;
        
        private static int _resetPrio = 0;
        private static int _activePrio = 10;


        public static Transform _wireModelTransform = null;
        
        
        private void Start()
        {
            //Set all static variables to the references given to the active gameobject
            _mainCamTransform = init_mainCamTransform;
            _resetPrio = init_resetPrio;
            _activePrio = init_activePrio;
            _player = init_player;
            _moveable = init_moveable;
            _pointOfInterest = init_pointOfInterest;
            _wire = init_wire;
            _groundEnemy = init_groundEnemy;
            _flyingEnemy = init_flyingEnemy;
            _spring = init_spring;
        }

        public static void DisableCameraMovement()
        {
            //Check for active camera
            //have a previous speed variable for x/y
            //set camera speed to 0
        }

        public static void EnableCameraMovement()
        {
            //check which is the active camera 
            //set its speed back to the previous speeds
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

            bool currentlyWire = false;
            if (_currentHackableCamera == _wire)
                currentlyWire = true;


            CinemachineVirtualCamera previousCamera = null;
            //Just as a default thing the camera is 100% changing so the previous needs to be reset
            if (_currentHackableCamera != null)
            {
                previousCamera = _currentHackableCamera;
            }

            
            switch (a_type)
            {
                case ObjectType.Player:
                    _player.Priority = 20;
                    if (currentlyWire)
                    {
                        Quaternion lookDirection = Quaternion.LookRotation(_wireModelTransform.forward);
                        
                        Vector3 eularAmount = lookDirection.eulerAngles;

                        Vector3 eularOffset = a_offset.rotation.eulerAngles;
                        eularOffset.y = eularAmount.y;
                        a_offset.rotation = Quaternion.Euler(eularOffset);
                        break;
                    }
                    else
                    {
                        float newYRot = _mainCamTransform.rotation.eulerAngles.y;
                        Vector3 eularOffset = a_offset.rotation.eulerAngles;
                        eularOffset.y = newYRot;
                        a_offset.rotation = Quaternion.Euler(eularOffset);
                        break;
                    }
                case ObjectType.Moveable:
                    _currentHackableCamera = _moveable;
                    //Sets the y rotation of the camera to be the previous cameras y rotation
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
                    float newYWire = _mainCamTransform.rotation.eulerAngles.y;
                    Vector3 offsetEularWire = a_offset.rotation.eulerAngles;
                    offsetEularWire.y = newYWire;
                    a_offset.rotation = Quaternion.Euler(offsetEularWire);
                    break;
                case ObjectType.GroundEnemy:
                    _currentHackableCamera = _groundEnemy;
                    break;
                case ObjectType.FlyingEnemy:
                    _currentHackableCamera = _flyingEnemy;
                    break;
                case ObjectType.Spring:
                    _currentHackableCamera = _spring;
                    float newYSpring = _mainCamTransform.rotation.eulerAngles.y;
                    Vector3 offsetEularSpring = a_offset.rotation.eulerAngles;
                    offsetEularSpring.y = newYSpring;
                    a_offset.rotation = Quaternion.Euler(offsetEularSpring);
                    break;
            }

            if (previousCamera != null)
                previousCamera.Priority = _resetPrio;

            if (a_offset != null)
            {
                _currentHackableCamera.LookAt = a_offset;
                _currentHackableCamera.Follow = a_offset;
            }
            _currentHackableCamera.Priority = _activePrio;
        }
    }
}