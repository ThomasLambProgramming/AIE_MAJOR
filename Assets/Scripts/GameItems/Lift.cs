using System;
using System.Collections;
using UnityEngine;
using Malicious.Core;
using UnityEditor;

namespace Malicious.GameItems
{
    public class Lift : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private Vector3 _topPosition = Vector3.zero;
        [SerializeField] private Vector3 _groundPosition = Vector3.zero;
        [SerializeField] private Transform _liftObject = null;
        
        private bool _wait = true;
        private bool _moveUp = false;
        private bool _moveDown = false;
        private float _timer = 0;
        
        void FixedUpdate()
        {
            if (_wait)
                return;

            if (_moveUp)
            {
                _timer += Time.deltaTime * _moveSpeed;
                _liftObject.position = Vector3.Lerp(_groundPosition, _topPosition, _timer);
                
                if (_timer >= 1)
                    _wait = true;
            }
            else if (_moveDown)
            {
                _timer -= Time.deltaTime * _moveSpeed;
                _liftObject.position = Vector3.Lerp(_groundPosition, _topPosition, _timer);
                
                if (_timer <= 0)
                    _wait = true;
            }

            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.transform.parent = _liftObject.transform;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.transform.parent = null;
            }
        }

        [ContextMenu("SetPositions")]
        public void SetPositions()
        {
            _groundPosition = _liftObject.position;
            _topPosition = _groundPosition;
            _topPosition.y += 3;
        }


        [ContextMenu("MoveUp")]
        public void MoveUp()
        {
            _moveUp = true;
            _moveDown = false;
            _wait = false;
        }

        [ContextMenu("MoveDown")]
        public void MoveToGround()
        {
            _moveUp = false;
            _moveDown = true;
            _wait = false;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_groundPosition, Vector3.one);
            Gizmos.DrawCube(_topPosition, Vector3.one);
        }
#endif
    }
}