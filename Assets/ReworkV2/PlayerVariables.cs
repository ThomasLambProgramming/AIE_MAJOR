using System;
using UnityEngine;

namespace Malicious.ReworkV2
{
    [Serializable]
    public class PlayerValues
    {
        [SerializeField] public Transform _cameraOffset = null;
        [HideInInspector] public Rigidbody _rigidbody = null;

        [HideInInspector] public IPlayerObject _currentInteract = null;
        
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

        //------Speed Variables----------------//
        public float _goNextWire = 0.2f;
        public float _wireSpeed = 10f;
        public float _rotateSpeed = 10f;
    }

    [Serializable]
    public class MoveObjValues
    {
        [SerializeField] public Rigidbody _rigidbody = null;
        [HideInInspector] public Transform _movementTransform = null;
        
        //------Speed Variables----------------//
        [SerializeField] public float _moveSpeed = 5f;
        [SerializeField] public float _spinSpeed = 5f;
        
        //------Input Variables----------------//
        [HideInInspector] public Vector2 _moveInput = Vector2.zero;
        [HideInInspector] public Vector2 _spinInput = Vector2.zero;
    }
    
}