using System;
using System.Collections.Generic;
using Cinemachine;
using Malicious.Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace Malicious.Core
{

    [Serializable]
    public class WireValues
    {
        //------Wire Variables-----------------//
        [SerializeField] public GameObject _wireModel = null;
        [SerializeField] public Transform _wireCameraOffset = null;
        [SerializeField] public List<Vector3> _wirePath = new List<Vector3>();
        
        //------Other Variables----------------//
        [HideInInspector] public int _pathIndex = 0;
        
        [HideInInspector] public Quaternion _rotationGoal = quaternion.identity;
        [HideInInspector] public bool _rotateObject = false;
        [HideInInspector] public Vector3 _startingDirection = Vector3.zero;
        
        public float _heightAngleAllowance = 0.6f;
        public float _wireLength = 5f;
        public int _wireCharges = 4;
        [HideInInspector] public int _chargesLeft = 0;
        
        public Vector3 _rigOffset = Vector3.zero;
        [HideInInspector] public bool _moveToEnd = false;
        [HideInInspector] public bool _takingInput = false;
        
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