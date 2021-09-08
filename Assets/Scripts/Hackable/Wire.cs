
using Malicious.Core;
using Malicious.Player;
using Malicious.Interfaces;

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Malicious.Hackable
{
    public class Wire : MonoBehaviour, IPlayerObject
    {
        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        
        //------Wire Variables-----------------//
        [SerializeField] private GameObject _wireModel = null;
        [SerializeField] private Transform _wireCameraOffset = null;
        [SerializeField] private List<Vector3> _wirePath = new List<Vector3>();
        
        //------Other Variables----------------//
        private int _pathIndex = 0;
        
        private Quaternion _rotationGoal = Quaternion.identity;
        private bool _rotateObject = false;
        private Vector3 _startingDirection = Vector3.zero;
        
        [SerializeField] private float _heightAngleAllowance = 0.6f;
        [SerializeField] private float _wireLength = 5f;
        [SerializeField] private int _wireCharges = 4;
        private int _chargesLeft = 0;
        
        private Vector3 _rigOffset = Vector3.zero;
        private bool _moveToEnd = false;
        private bool _takingInput = false;
        
        //------Speed Variables----------------//
        [SerializeField] private float _goNextWire = 0.2f;
        [SerializeField] private float _wireSpeed = 10f;
        [SerializeField] private float _rotateSpeed = 10f;
        
        //------Debug Variables----------------//
        [SerializeField] private bool _showPath = true;
       
        private void Start()
        {
            //This will need to be changed to allow for saving the input
            _chargesLeft = _wireCharges;
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
        }

        public void OnHackEnter()
        {
            _nodeRenderer.material = _hackedMaterial;
            _wireModel = PlayerController.PlayerControl._wireModel;
            _wireCameraOffset = PlayerController.PlayerControl._wireModelOffset;
            _wireModel.SetActive(true);

            //THIS NEEDS TO CHANGE TO A DEFAULT PARAMETER SO IT CAN BE STARTED WITH 1 WIRE
            Vector3 directionToNode = (_wirePath[1] - _wirePath[0]).normalized;
            _startingDirection = directionToNode;
            Quaternion newRotation = Quaternion.LookRotation(directionToNode);
            
            _wireModel.transform.rotation = newRotation;
            _wireModel.transform.position = _wirePath[0];

            if (_wirePath.Count > 0)
            {
                _takingInput = false;
                _moveToEnd = true;
            }
            else
            {
                _takingInput = true;
                _moveToEnd = false;
            }

            EnableInput();
        }

        public void OnHackExit()
        {
            _pathIndex = 0;
            _rotationGoal = Quaternion.identity;
            _wireModel.SetActive(false);

            _takingInput = false;
            _moveToEnd = false;
            DisableInput();
            _nodeRenderer.material = _defaultMaterial;
        }

        public void Tick()
        {
            
        }
        public void FixedTick()
        {
            if (_takingInput)
                return;
            if (_moveToEnd)
                MoveToEndOfWire();
            else
            {
                SetToPlayer();
            }
        }

        private void MoveToEndOfWire()
        {
            bool moveRotOccuring = false;
            if (Vector3.SqrMagnitude(_wirePath[_pathIndex] - _wireModel.transform.position) >
                _goNextWire)
            {
                moveRotOccuring = true;
                Vector3 currentWirePos = _wireModel.transform.position;
                currentWirePos = currentWirePos +
                                 (_wirePath[_pathIndex] - _wireModel.transform.position)
                                 .normalized *
                                 (Time.deltaTime * (_wireSpeed));
                _wireModel.transform.position = currentWirePos;
            }
            if (_rotateObject)
            {
                moveRotOccuring = true;
                _wireModel.transform.rotation = Quaternion.RotateTowards(
                    _wireModel.transform.rotation,
                    _rotationGoal,
                    _rotateSpeed);
                if (_wireModel.transform.rotation == _rotationGoal)
                    _rotateObject = false;
                
            }

            if (moveRotOccuring == false) 
            {
                if (_pathIndex < _wirePath.Count - 1)
                {
                    _pathIndex++;
                    Vector3 newDirection = (_wirePath[_pathIndex] - _wirePath[_pathIndex - 1]).normalized;

                    if (Vector3.Dot(newDirection, Vector3.up) < _heightAngleAllowance &&
                        Vector3.Dot(newDirection, Vector3.down) < _heightAngleAllowance)
                    {
                        _rotationGoal = Quaternion.LookRotation(
                            newDirection, Vector3.up);
                        _rotateObject = true;
                    }
                }
                else
                {
                    if (_chargesLeft > 0)
                    {
                        _takingInput = true;
                        _moveToEnd = false;
                    }
                    else
                    {
                        //we want to exit
                        _moveToEnd = false;
                        _takingInput = false;
                    }
                }
            }
        }

        private void InputProcessing(InputAction.CallbackContext a_context)
        {
            if (!_takingInput)
                return;
            
            Vector2 input = a_context.ReadValue<Vector2>();
            
            //player needs to move the stick a set amount
            if (input.magnitude < 0.4f)
                return;

            Vector3 forwardDirection = _wireModel.transform.forward;
            Vector3 rightDirection = _wireModel.transform.right;

            if (Vector2.Dot(input, Vector2.up) > 0.8f)
            {
                AddPoint(forwardDirection);
            }
            if (Vector2.Dot(input, Vector2.down) > 0.8f)
            {
                if (_pathIndex == 0)
                    return;
                _takingInput = false;
                _chargesLeft++;
                _moveToEnd = true;
                _pathIndex--;
                
                _wirePath.RemoveAt(_wirePath.Count - 1);

                if (_pathIndex > 0)
                {
                    Vector3 previousDirection = (_wirePath[_pathIndex] - _wirePath[_pathIndex - 1]).normalized;

                    if (Vector3.Dot(previousDirection, Vector3.up) < _heightAngleAllowance &&
                        Vector3.Dot(previousDirection, Vector3.down) < _heightAngleAllowance)
                    {
                        _rotationGoal = Quaternion.LookRotation(previousDirection,
                            Vector3.up);
                        _rotateObject = true;
                    }
                }
                else
                {
                    _rotationGoal = Quaternion.LookRotation(
                        (_startingDirection),
                        Vector3.up);
                    _rotateObject = true;
                }
            }
            if (Vector2.Dot(input, Vector2.left) > 0.8f)
            {
                AddPoint(-rightDirection);
                _rotateObject = true;
            }
            if (Vector2.Dot(input, Vector2.right) > 0.8f)
            {
                AddPoint(rightDirection);
                _rotateObject = true;
            }
        }

        private void AddPoint(Vector3 a_direction)
        {
            if (CheckDirection(a_direction))
            {
                Vector3 newWirePoint = _wirePath[_wirePath.Count - 1];
                Vector3 directionAdd = a_direction;
                
                directionAdd = directionAdd.normalized;
                directionAdd *= _wireLength;
                
                newWirePoint += directionAdd;

                if (CheckPoint(newWirePoint))
                {
                    _takingInput = false;
                    _moveToEnd = true;
                    _chargesLeft--;
                    
                    _wirePath.Add(newWirePoint);
                }
            }
        }

        //Check all other points in the wire list to not allow overlap
        private bool CheckPoint(Vector3 a_position)
        {
            bool isValid = true;
            foreach (var location in _wirePath)
            {
                if (Vector3.SqrMagnitude(location - a_position) < 2)
                    isValid = false;
            }

            if (isValid)
                return true;
            
            return false;
        }
        //if it collides with anything other than wire layer a new wire is not allowed 
        private bool CheckDirection(Vector3 a_direction)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                _wireModel.transform.position, 
                a_direction, 
                out hit, 
                _wireLength,
                ~(1 << 8)))
            {
                if (hit.collider != null)
                    Debug.Log(hit.collider.gameObject.name);    
                return false;
            }
            
            return true;
        }
        

        private void SetToPlayer()
        {
            PlayerController.PlayerControl.ResetToPlayer();
        }
        
        public OffsetContainer GiveOffset()
        {
            OffsetContainer temp = new OffsetContainer();
            temp._offsetTransform = _wireCameraOffset;
            temp._rigOffset = _rigOffset;
            return temp;
        }
        
        public bool RequiresTruePlayerOffset() => false;
        public void SetOffset(Transform a_offset) => _wireCameraOffset = a_offset;
        public void OnHackValid()
        {
            _nodeRenderer.material = _hackValidMaterial;
        }
        public void OnHackFalse()
        {
            _nodeRenderer.material = _defaultMaterial;
        }

        public ObjectType ReturnType() => ObjectType.Wire;
        

        private void UpDirection(InputAction.CallbackContext a_context)
        {
            if (_takingInput) 
                AddPoint(_wireModel.transform.up);
        }
        private void DownDirection(InputAction.CallbackContext a_context)
        {
            if (_takingInput) 
                AddPoint(-_wireModel.transform.up);
        }
        private void ExitWireInput(InputAction.CallbackContext a_context)
        {
            SetToPlayer();
        }
        private void EnableInput()
        {
            GlobalData.InputManager.Player.Movement.performed += InputProcessing;
            GlobalData.InputManager.Player.Jump.performed += UpDirection;
            GlobalData.InputManager.Player.Down.performed += DownDirection;
            GlobalData.InputManager.Player.Interaction.performed += ExitWireInput;
        }
        private void DisableInput()
        {
            GlobalData.InputManager.Player.Movement.performed -= InputProcessing;
            GlobalData.InputManager.Player.Jump.performed -= UpDirection;
            GlobalData.InputManager.Player.Down.performed -= DownDirection;
            GlobalData.InputManager.Player.Interaction.performed -= ExitWireInput;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_showPath)
            {
                foreach (var point in _wirePath)
                {
                    Gizmos.DrawSphere(point, 1);
                }
            }
        }
        //Add a path onto the end just makes it easier to make a path not needing to copy paste 
        //positions
        [ContextMenu("AddPathPoint")]
        public void AddPathPoint()
        {
            if (_wirePath.Count > 0)
            {
                Vector3 newPoint = _wirePath[_wirePath.Count - 1];
                _wirePath.Add(newPoint);
            }
            else
            {
                _wirePath = new List<Vector3>();
                _wirePath.Add(gameObject.transform.position);
            }
        }
#endif
    }
}