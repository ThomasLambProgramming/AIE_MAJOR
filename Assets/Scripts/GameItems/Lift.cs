using System.Collections;
using UnityEngine;
using Malicious.Core;
using UnityEditor;

namespace Malicious.GameItems
{
    public class Lift : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        private Vector3 _targetLocation = Vector3.zero;

        [SerializeField] private Vector3 _topPosition = Vector3.zero;
        [SerializeField] private Vector3 _groundPosition = Vector3.zero;
        
        private bool _wait = true;
        
        private bool _moveUp = false;
        private bool _moveDown = false;

        private float _timer = 0;
        
        void Start()
        {
            _targetLocation = _topPosition;
        }
        void FixedUpdate()
        {
            if (_wait)
                return;

            if (_moveUp)
            {
                _timer += Progress.TimeDisplayMode.
            }
            else if (_moveDown)
            {
                
            }
        }

        public void MoveUp()
        {
            _moveUp = true;
            _moveDown = false;
            _timer = 0;
        }

        public void MoveToGround()
        {
            _moveUp = false;
            _moveDown = true;
            _timer = 0;
        }
    }
}