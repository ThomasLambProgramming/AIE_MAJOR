using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.Hackable
{
    [RequireComponent(typeof(HackableField))]
    public class Wire : BasePlayer
    {
        [SerializeField] private GameObject _wireModel = null;
        [SerializeField] private Transform _wireCameraOffset = null;
        [SerializeField] private List<Vector3> _wirePath = new List<Vector3>();
        //since it will be a prefab probably just make it have a script that auto plays the shader
        //for it to dissolve in to avoid any logic being needed
        [SerializeField] private GameObject _wirePrefab = null;
        [SerializeField] private Vector3 _startingCameraDirection = Vector3.zero;
        
        //------Other Variables----------------//
        [SerializeField] private Vector3 _startingDirection = Vector3.zero;
        [SerializeField] private float _heightAngleAllowance = 0.6f;
        [SerializeField] private float _wireLength = 5f;
        [SerializeField] private int _wireCharges = 4;
        [SerializeField] private float _stoppingDistance = 0.2f;
        [SerializeField] private float _dissolveTime = 1f;
        [SerializeField] private LayerMask _wireStopMask = ~0;
        [SerializeField] private float _maxTimeForExit = 0.4f;
        [SerializeField] private Vector3 _launchDirection = Vector3.zero;
        [SerializeField] private float _launchForce = 0;
        [SerializeField] private float _resetSpeed = 2f;

        private bool _dissolveRunning = false;
        private Vector3 _resetAmount = Vector3.zero;
        private bool _resetting = false;
        private bool _holdingInteractButton = false;
        private float _holdTimer = 0;
        
        private Quaternion _rotationGoal = Quaternion.identity;
        private bool _rotateObject = false;
        private bool _moveToEnd = false;
        private bool _takingInput = false;
        private int _pathIndex = 0;
        private int _chargesLeft = 0;

        [SerializeField] private List<WireModelDissolve> _dissolveWires = new List<WireModelDissolve>();
        
        //------Debug Variables----------------//
        [SerializeField] private bool _showPath = true;
        [SerializeField] private Vector3 _pointSize = new Vector3(0.4f, 0.4f, 0.4f);
        [SerializeField] private bool _showDirection = true;
        //[SerializeField] private bool _showLaunchDirection = true;
        private void Start()
        {
            _chargesLeft = _wireCharges;
            _pathIndex = 0;
        }

        protected override void Tick()
        {
            if (_holdingInteractButton)
                HoldInputTimer();
        }

        protected override void FixedTick()
        {
            if (_dissolveRunning)
                return;
            
            if (_resetting)
            {
                Vector3 direction = (_wirePath[0] - _wireModel.transform.position);
                if (direction.sqrMagnitude < 0.005f)
                {
                    _wireModel.transform.position = _wirePath[0];
                    _resetting = false;
                    _wireModel.SetActive(true);
                    _chargesLeft = _wireCharges;
                    _takingInput = true;
                    _moveToEnd = false;
                    _pathIndex = 0;
                    return;
                }
                
                _wireModel.transform.position += _resetAmount * (Time.deltaTime * _resetSpeed);
                
            }
            else if (_takingInput)
                return;
            if (_moveToEnd)
                MoveToEndOfWire();
        }

        public override void OnHackEnter()
        {
            CameraController.ChangeCamera(ObjectType.Wire, _wireCameraOffset);
            base.OnHackEnter();
            _wireModel.SetActive(true);
            _wireModel.transform.rotation = Quaternion.LookRotation(_startingCameraDirection);
            _wireModel.transform.position = _wirePath[0];
            if (_wirePath.Count > 1)
            {
                _moveToEnd = true;
                _takingInput = false;
            }
            else
            {
                _moveToEnd = false;
                _takingInput = true;
            }
        }

        public override void OnHackExit()
        {
            base.OnHackExit();
            _pathIndex = 0;
            _rotationGoal = Quaternion.identity;
            _wireModel.SetActive(false);
            _takingInput = false;
            _moveToEnd = false;
            ResetPath();
        }

        private void InsideWireHoldOptionActivate()
        {
            if (_wirePath.Count == 1)
                return;
            
            ResetPath();
            _wireModel.SetActive(false);
        }
        public override void HoldOptionActivate()
        {
            Vector3 startingPos = _wirePath[0];
            _wirePath.Clear();
            _wirePath = new List<Vector3>();
            _wirePath.Add(startingPos);
            _pathIndex = 0;
            _chargesLeft = _wireCharges;
            foreach (var wire in _dissolveWires)
            {
                wire.DissolveOut(true);
            }
            _dissolveWires.Clear();
        }

        private void ResetPath()
        {
            Vector3 startingPos = _wirePath[0];
            _wirePath.Clear();
            _wirePath = new List<Vector3>();
            _wirePath.Add(startingPos);
            _wireModel.transform.rotation = Quaternion.LookRotation(_startingDirection);
            _resetAmount = _wirePath[0] - _wireModel.transform.position;
            _resetting = true;
            foreach (var wire in _dissolveWires)
            {
                wire.DissolveOut(true);
            }
            _dissolveWires.Clear();
        }
        private void MoveToEndOfWire()
        {
            bool moveRotOccuring = false;
            if (Vector3.SqrMagnitude(_wirePath[_pathIndex] - _wireModel.transform.position) >
                _stoppingDistance)
            {
                moveRotOccuring = true;
                Vector3 currentWirePos = _wireModel.transform.position;
                currentWirePos = currentWirePos +
                                 (_wirePath[_pathIndex] - _wireModel.transform.position)
                                 .normalized *
                                 (Time.deltaTime * (_moveSpeed));
                _wireModel.transform.position = currentWirePos;
            }
            else
            {
                _wireModel.transform.position = _wirePath[_pathIndex];
            }
            if (_rotateObject)
            {
                moveRotOccuring = true;
                _wireModel.transform.rotation = Quaternion.RotateTowards(
                    _wireModel.transform.rotation,
                    _rotationGoal,
                    _spinSpeed);
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
        private void AddPoint(Vector3 a_direction)
        {
            if (_chargesLeft <= 0)
                return;

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
                    GameObject accessVar = 
                        Instantiate(_wirePrefab, 
                            _wirePath[_wirePath.Count - 2], 
                            Quaternion.LookRotation(a_direction));
                    Vector3 scaleBuffer = accessVar.transform.localScale;
                    scaleBuffer.z = _wireLength;
                    accessVar.transform.localScale = scaleBuffer;
                    
                    //this is for the resetting to be able to dissolve them before deleting
                    _dissolveWires.Add(accessVar.GetComponentInChildren<WireModelDissolve>());
                    StartCoroutine(WaitForDissolve());
                }
            }
        }

        private IEnumerator WaitForDissolve()
        {
            _dissolveRunning = true;
            bool waiting = true;
            float timer = 0;
            while (waiting)
            {
                timer += Time.deltaTime;
                if (timer > _dissolveTime)
                    waiting = false;
                yield return null;
            }

            _dissolveRunning = false;
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
                _wireStopMask))
            {
                if (hit.collider != null)
                    Debug.Log(hit.collider.gameObject.name);    
                return false;
            }
            
            return true;
        }

        private void RemoveInputEnter(InputAction.CallbackContext a_context)
        {
            if (_moveToEnd)
                return;

            if (_pathIndex == 0)
                return;
            _takingInput = false;
            _chargesLeft++;
            _moveToEnd = true;
            _pathIndex--;
                
            _wirePath.RemoveAt(_wirePath.Count - 1);
            _dissolveWires[_wirePath.Count - 1].DissolveOut(false);
            StartCoroutine(DeleteWire(_wirePath.Count - 1));
                
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
                    (_startingCameraDirection),
                    Vector3.up);
                _rotateObject = true;
            }
        }
        protected override void MoveInputEnter(InputAction.CallbackContext a_context)
        {
            //if we are moving then we dont want to be able to take in input
            if (_moveToEnd)
                return;
            
            Vector2 input = a_context.ReadValue<Vector2>();
            
            //player needs to move the stick a set amount
            if (input.magnitude < 0.4f)
                return;

            Vector3 forwardDirection = _wireModel.transform.forward;
            Vector3 rightDirection = _wireModel.transform.right;

            
            if (Vector2.Dot(input, Vector2.down) > 0.8f)
            {
                AddPoint(-forwardDirection);
                _rotateObject = true;
            }
            else if (Vector2.Dot(input, Vector2.up) > 0.8f)
                AddPoint(forwardDirection);
            else if (Vector2.Dot(input, Vector2.left) > 0.8f)
            {
                AddPoint(-rightDirection);
                _rotateObject = true;
            }
            else if (Vector2.Dot(input, Vector2.right) > 0.8f)
            {
                AddPoint(rightDirection);
                _rotateObject = true;
            }
        }
        protected override void JumpInputEnter(InputAction.CallbackContext a_context)
        {
            if (_chargesLeft == 0)
            {
                ReturnToPlayer();
            }
            else if (_takingInput) 
                AddPoint(_wireModel.transform.up);
        }
        protected override void DownInputEnter(InputAction.CallbackContext a_context)
        {
            if (_takingInput) 
                AddPoint(-_wireModel.transform.up);
        }

        private bool _interactionEntered = false;
        protected override void InteractionInputEnter(InputAction.CallbackContext a_context)
        {
            _interactionEntered = true;
            //Set to player
            _holdingInteractButton = true;
            _holdTimer = 0;
        }
        protected override void InteractionInputExit(InputAction.CallbackContext a_context)
        {
            //Since the e key will enter to begin with the player can hold and use the exit before starting
            //so its possible to enter and be forced to hold the input key
            if (!_interactionEntered)
                return;
            _interactionEntered = false;
            //Set to player
            _holdingInteractButton = false;

            if (_holdTimer < _maxTimeForExit)
            {
                ReturnToPlayer();
            }
            _holdTimer = 0;
        }

        private void ReturnToPlayer()
        {
            Vector3 shootDirection = Vector3.zero;
            if (_wirePath.Count == 1)
            {
                shootDirection = _startingDirection;
                shootDirection.y = _launchDirection.y;
                shootDirection = shootDirection.normalized;
            }
            else
            {
                shootDirection = _wirePath[_pathIndex] - _wirePath[_pathIndex - 1];
                shootDirection = shootDirection.normalized;
                float dotResult = Vector3.Dot(shootDirection, Vector3.up);
                if (dotResult > -0.7f && dotResult < 0.7f)
                {
                    shootDirection.y = _launchDirection.y;
                }
                shootDirection = shootDirection.normalized;
            }

            _player.transform.position = _wireModel.transform.position + _wireModel.transform.forward;
            Vector3 wireModelEular = _wireModel.transform.rotation.eulerAngles;
            Vector3 playerEular = _player.transform.rotation.eulerAngles;
            
            playerEular.y = wireModelEular.y;
            _player.transform.rotation = Quaternion.Euler(playerEular);
            
            _player.LaunchPlayer(shootDirection * _launchForce);
            _player.OnHackEnter();
            OnHackExit();
        }
        
        private void HoldInputTimer()
        {
            _holdTimer += Time.deltaTime;

            if (_holdTimer >= _holdChargeTime)
            {
                _holdingInteractButton = false;
                InsideWireHoldOptionActivate();
            }
        }

        private IEnumerator DeleteWire(int a_index)
        {
            yield return new WaitForSeconds(1f);
            _dissolveWires.RemoveAt(a_index);
        }
        
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_wirePath.Count > 0)
            {
                if (_showDirection)
                {
                    Gizmos.DrawLine(_wirePath[0], _wirePath[0] + _startingDirection.normalized * 4f);
                    Gizmos.DrawLine(transform.position, transform.position + _startingCameraDirection * 2);   
                }

                if (_showPath)
                {
                    foreach (var VARIABLE in _wirePath)
                    {
                        Gizmos.DrawCube(VARIABLE, _pointSize);
                    }
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
        [ContextMenu("CalculateDirection")]
        public void CalculateDirection()
        {
            _startingDirection = (_wirePath[0] - transform.position).normalized;
        }
        #endif

        protected override void EnableInput()
        {
            base.EnableInput();
            GlobalData.InputManager.Player.RemoveWire.performed += RemoveInputEnter;
        }

        protected override void DisableInput()
        {
            base.DisableInput();
            GlobalData.InputManager.Player.RemoveWire.performed -= RemoveInputEnter;
        }
    }
    
}