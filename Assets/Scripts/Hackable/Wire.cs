
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
        [SerializeField] private WireValues _values = new WireValues();
        
        
        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        
        
       
        private void Start()
        {
            //This will need to be changed to allow for saving the input
            _values._chargesLeft = _values._wireCharges;
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
        }

        public void OnHackEnter()
        {
            _nodeRenderer.material = _hackedMaterial;
            _values._wireModel = PlayerController.PlayerControl._wireModel;
            _values._wireCameraOffset = PlayerController.PlayerControl._wireModelOffset;
            _values._wireModel.SetActive(true);

            //THIS NEEDS TO CHANGE TO A DEFAULT PARAMETER SO IT CAN BE STARTED WITH 1 WIRE
            Vector3 directionToNode = (_values._wirePath[1] - _values._wirePath[0]).normalized;
            _values._startingDirection = directionToNode;
            Quaternion newRotation = Quaternion.LookRotation(directionToNode);
            
            _values._wireModel.transform.rotation = newRotation;
            _values._wireModel.transform.position = _values._wirePath[0];

            if (_values._wirePath.Count > 0)
            {
                _values._takingInput = false;
                _values._moveToEnd = true;
            }
            else
            {
                _values._takingInput = true;
                _values._moveToEnd = false;
            }

            EnableInput();
        }

        public void OnHackExit()
        {
            _values._pathIndex = 0;
            _values._rotationGoal = Quaternion.identity;
            _values._wireModel.SetActive(false);

            _values._takingInput = false;
            _values._moveToEnd = false;
            DisableInput();
            _nodeRenderer.material = _defaultMaterial;
        }

        public void Tick()
        {
            
        }
        public void FixedTick()
        {
            if (_values._takingInput)
                return;
            if (_values._moveToEnd)
                MoveToEndOfWire();
            else
            {
                SetToPlayer();
            }
        }

        private void MoveToEndOfWire()
        {
            bool moveRotOccuring = false;
            if (Vector3.SqrMagnitude(_values._wirePath[_values._pathIndex] - _values._wireModel.transform.position) >
                _values._goNextWire)
            {
                moveRotOccuring = true;
                Vector3 currentWirePos = _values._wireModel.transform.position;
                currentWirePos = currentWirePos +
                                 (_values._wirePath[_values._pathIndex] - _values._wireModel.transform.position)
                                 .normalized *
                                 (Time.deltaTime * (_values._wireSpeed));
                _values._wireModel.transform.position = currentWirePos;
            }
            if (_values._rotateObject)
            {
                moveRotOccuring = true;
                _values._wireModel.transform.rotation = Quaternion.RotateTowards(
                    _values._wireModel.transform.rotation,
                    _values._rotationGoal,
                    _values._rotateSpeed);
                if (_values._wireModel.transform.rotation == _values._rotationGoal)
                    _values._rotateObject = false;
                
            }

            if (moveRotOccuring == false) 
            {
                if (_values._pathIndex < _values._wirePath.Count - 1)
                {
                    _values._pathIndex++;
                    Vector3 newDirection = (_values._wirePath[_values._pathIndex] - _values._wirePath[_values._pathIndex - 1]).normalized;

                    if (Vector3.Dot(newDirection, Vector3.up) < _values._heightAngleAllowance &&
                        Vector3.Dot(newDirection, Vector3.down) < _values._heightAngleAllowance)
                    {
                        _values._rotationGoal = Quaternion.LookRotation(
                            newDirection, Vector3.up);
                        _values._rotateObject = true;
                    }
                }
                else
                {
                    if (_values._chargesLeft > 0)
                    {
                        _values._takingInput = true;
                        _values._moveToEnd = false;
                    }
                    else
                    {
                        //we want to exit
                        _values._moveToEnd = false;
                        _values._takingInput = false;
                    }
                }
            }
        }

        private void InputProcessing(InputAction.CallbackContext a_context)
        {
            if (!_values._takingInput)
                return;
            
            Vector2 input = a_context.ReadValue<Vector2>();
            
            //player needs to move the stick a set amount
            if (input.magnitude < 0.4f)
                return;

            Vector3 forwardDirection = _values._wireModel.transform.forward;
            Vector3 rightDirection = _values._wireModel.transform.right;

            if (Vector2.Dot(input, Vector2.up) > 0.8f)
            {
                AddPoint(forwardDirection);
            }
            if (Vector2.Dot(input, Vector2.down) > 0.8f)
            {
                if (_values._pathIndex == 0)
                    return;
                _values._takingInput = false;
                _values._chargesLeft++;
                _values._moveToEnd = true;
                _values._pathIndex--;
                
                _values._wirePath.RemoveAt(_values._wirePath.Count - 1);

                if (_values._pathIndex > 0)
                {
                    Vector3 previousDirection = (_values._wirePath[_values._pathIndex] - _values._wirePath[_values._pathIndex - 1]).normalized;

                    if (Vector3.Dot(previousDirection, Vector3.up) < _values._heightAngleAllowance &&
                        Vector3.Dot(previousDirection, Vector3.down) < _values._heightAngleAllowance)
                    {
                        _values._rotationGoal = Quaternion.LookRotation(previousDirection,
                            Vector3.up);
                        _values._rotateObject = true;
                    }
                }
                else
                {
                    _values._rotationGoal = Quaternion.LookRotation(
                        (_values._startingDirection),
                        Vector3.up);
                    _values._rotateObject = true;
                }
            }
            if (Vector2.Dot(input, Vector2.left) > 0.8f)
            {
                AddPoint(-rightDirection);
                _values._rotateObject = true;
            }
            if (Vector2.Dot(input, Vector2.right) > 0.8f)
            {
                AddPoint(rightDirection);
                _values._rotateObject = true;
            }
        }

        private void AddPoint(Vector3 a_direction)
        {
            if (CheckDirection(a_direction))
            {
                Vector3 newWirePoint = _values._wirePath[_values._wirePath.Count - 1];
                Vector3 directionAdd = a_direction;
                
                directionAdd = directionAdd.normalized;
                directionAdd *= _values._wireLength;
                
                newWirePoint += directionAdd;

                if (CheckPoint(newWirePoint))
                {
                    _values._takingInput = false;
                    _values._moveToEnd = true;
                    _values._chargesLeft--;
                    
                    _values._wirePath.Add(newWirePoint);
                }
            }
        }

        //Check all other points in the wire list to not allow overlap
        private bool CheckPoint(Vector3 a_position)
        {
            bool isValid = true;
            foreach (var location in _values._wirePath)
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
                _values._wireModel.transform.position, 
                a_direction, 
                out hit, 
                _values._wireLength,
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
            PlayerController.PlayerControl.ResetToPlayer(
                _values._wireModel.transform.position,
                _values._wireModel.transform.rotation);
        }
        
        public OffsetContainer GiveOffset()
        {
            OffsetContainer temp = new OffsetContainer();
            temp._offsetTransform = _values._wireCameraOffset;
            temp._rigOffset = _values._rigOffset;
            return temp;
        }
        
        public bool RequiresTruePlayerOffset() => false;
        public void SetOffset(Transform a_offset) => _values._wireCameraOffset = a_offset;
        public void OnHackValid()
        {
            _nodeRenderer.material = _hackValidMaterial;
        }
        public void OnHackFalse()
        {
            _nodeRenderer.material = _defaultMaterial;
        }
        private void UpDirection(InputAction.CallbackContext a_context)
        {
            if (_values._takingInput) 
                AddPoint(_values._wireModel.transform.up);
        }
        private void DownDirection(InputAction.CallbackContext a_context)
        {
            if (_values._takingInput) 
                AddPoint(-_values._wireModel.transform.up);
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
            if (_values._showPath)
            {
                foreach (var point in _values._wirePath)
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
            if (_values._wirePath.Count > 0)
            {
                Vector3 newPoint = _values._wirePath[_values._wirePath.Count - 1];
                _values._wirePath.Add(newPoint);
            }
            else
            {
                _values._wirePath = new List<Vector3>();
                _values._wirePath.Add(gameObject.transform.position);
            }
        }
#endif
    }
}