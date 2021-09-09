using System;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.ReworkMk3
{
    [RequireComponent(typeof(HackableField))]
    public class Wire : BasePlayer
    {
        [SerializeField] private GameObject _wireModel = null;
        [SerializeField] private Transform _wireCameraOffset = null;
        [SerializeField] private List<Vector3> _wirePath = new List<Vector3>();
        
        //------Other Variables----------------//
        [SerializeField] private Vector3 _startingDirection = Vector3.zero;
        [SerializeField] private float _heightAngleAllowance = 0.6f;
        [SerializeField] private float _wireLength = 5f;
        [SerializeField] private int _wireCharges = 4;
        [SerializeField] private float _goNextWire = 0.2f;
        
        private Quaternion _rotationGoal = Quaternion.identity;
        private bool _rotateObject = false;
        private bool _moveToEnd = false;
        private bool _takingInput = false;
        private int _pathIndex = 0;
        private int _chargesLeft = 0;
        
        
        //------Debug Variables----------------//
        [SerializeField] private bool _showPath = true;
        [SerializeField] private bool _showDirection = true;
        [SerializeField] private Vector3 _pointSize = new Vector3(0.4f, 0.4f, 0.4f);

        private void Start()
        {
            _chargesLeft = _wireCharges;
            _wireModel.transform.rotation = Quaternion.LookRotation(_startingDirection);
            _wireModel.transform.position = _wirePath[0];
        }

        public override void OnHackEnter()
        {
            base.OnHackEnter();
            
        }

        public override void OnHackExit()
        {
            base.OnHackExit();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_showDirection)
                Gizmos.DrawLine(_wirePath[0], _wirePath[0] + _startingDirection.normalized * 4f);

            if (_showPath)
            {
                foreach (var VARIABLE in _wirePath)
                {
                    Gizmos.DrawCube(VARIABLE, _pointSize);
                }
            }
        }
        #endif
    }
    
}