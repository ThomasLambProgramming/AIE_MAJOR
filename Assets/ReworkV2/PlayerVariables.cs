using System;
using System.Collections.Generic;
using Malicious.Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace Malicious.ReworkV2
{
    [Serializable]
    public class PlayerValues
    {
        [SerializeField] public Transform _cameraOffset = null;
        [SerializeField] public Vector3 _rigOffset = Vector3.zero;
        [HideInInspector] public Rigidbody _rigidbody = null;

        //------Hacking Variables--------------//
            //Current hackable movement
        [HideInInspector] public IPlayerObject _currentInteract = null;
        [HideInInspector] public GameObject _currentInteractObj = null;
            //Current hackable control panel/lever
        [HideInInspector] public IHackable _currentHackable = null;
        [HideInInspector] public GameObject _currentHackableObj = null;
        
        [HideInInspector] public bool _canInteract = false;
        [HideInInspector] public bool _canHackable = false;
        [SerializeField] public float _dotAllowance = 0.8f;
        
        //------Speed Variables----------------//
        [SerializeField] public float _moveSpeed = 5f;
        [SerializeField] public float _jumpForce = 10f;
        [SerializeField] public float _spinSpeed = 5f;
        
        
        //------Jumping Variables--------------//
        [HideInInspector] public bool _canJump = true;
        [HideInInspector] public bool _hasDoubleJumped = false;
        [HideInInspector] public bool _holdingJump = false;
        public Transform _groundCheck = null;
        
        
        //------Animator Variables-------------//
        [SerializeField] public float _animationSwapSpeed = 3f;
        [HideInInspector] public Animator _playerAnimator = null;
        [HideInInspector] public Vector2 _currentAnimationVector = Vector2.zero;
        public readonly int _xPos = Animator.StringToHash("XPos");
        public readonly int _yPos = Animator.StringToHash("YPos");
        public readonly int _jumping = Animator.StringToHash("Jumping");
        
        //------Input Variables----------------//
        [HideInInspector] public Vector2 _moveInput = Vector2.zero;
        [HideInInspector] public Vector2 _spinInput = Vector2.zero;
    }
    
    [Serializable]
    public class WireValues
    {
        //------Wire Variables-----------------//
        [SerializeField] public GameObject _wireModel = null;
        [SerializeField] public Transform _wireCameraOffset = null;
        [SerializeField] public List<Vector3> _wirePath = new List<Vector3>();
        
        //------Other Variables----------------//
        public int _pathIndex = 0;
        
        public Quaternion _rotationGoal = quaternion.identity;
        public bool _rotateObject = false;

        public float _wireLength = 5f;
        public int _wireCharges = 4;

        public bool takingInput = false;
        
        //------Speed Variables----------------//
        public float _goNextWire = 0.2f;
        public float _wireSpeed = 10f;
        public float _rotateSpeed = 10f;
        
        //------Debug Variables----------------//
        public bool _showPath = true;
    }

    [Serializable]
    public class MoveObjValues
    {
        [SerializeField] public Rigidbody _rigidbody = null;
        [SerializeField] public Transform _cameraOffset = null;
        [SerializeField] public Vector3 _rigOffset = Vector3.zero;
        [HideInInspector] public Transform _movementTransform = null;
        
        //------Speed Variables----------------//
        [SerializeField] public float _moveSpeed = 5f;
        [SerializeField] public float _spinSpeed = 5f;
        
        //------Input Variables----------------//
        [HideInInspector] public Vector2 _moveInput = Vector2.zero;
        [HideInInspector] public Vector2 _spinInput = Vector2.zero;
    }
    
}