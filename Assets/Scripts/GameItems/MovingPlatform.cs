using System;
using System.Collections;
using Malicious.Core;
using UnityEngine;

namespace Malicious.GameItems
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private Vector3 _targetLocation = Vector3.zero;
        private Vector3 _startLocation = Vector3.zero;
        
        [SerializeField] private bool _waitPlayer = false;
        [SerializeField] private bool _waitForTime = false;
        [SerializeField] private float _waitTime = 3f;

        private float _timer = 0;
        private bool waiting = false;
        private bool _moveToTarget = true;
        
        private Vector3 _sceneStartLocation = Vector3.zero;
        
        void Start()
        {
            GameEventManager.PlayerDead += ResetToStart;
            _startLocation = transform.position;
            _sceneStartLocation = transform.position;
            
            if (_waitPlayer)
                waiting = true;
        }

        [ContextMenu("SetPositions")]
        public void MakePositions()
        {
            _startLocation = transform.position;
            _targetLocation = transform.position;
        }
        
        
        void FixedUpdate()
        {
            if (waiting)
                return;

            if (_moveToTarget)
            {
                if (_timer < 1)
                {
                    _timer += Time.deltaTime * _moveSpeed;
                    transform.position = Vector3.Lerp(_startLocation, _targetLocation, _timer);
                }
                else
                {
                    _timer = 1;
                    if (_waitForTime)
                    {
                        StartCoroutine(WaitAtPoint());
                        waiting = true;   
                    }
                    else
                        waiting = true;
                    
                    _moveToTarget = false;
                }
            }
            else
            {
                if (_timer > 0)
                {
                    _timer -= Time.deltaTime * _moveSpeed;
                    transform.position = Vector3.Lerp(_startLocation, _targetLocation, _timer);
                }
                else
                {
                    if (_waitForTime)
                    {
                        StartCoroutine(WaitAtPoint());
                        waiting = true;
                    }
                    else
                        waiting = true;
                    
                    _timer = 0;
                    _moveToTarget = true;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = this.transform;
                if (_waitPlayer && waiting)
                {
                    waiting = false;
                }
            }
        }
        void ResetToStart()
        {
            transform.position = _sceneStartLocation;
        }

        IEnumerator WaitAtPoint()
        {
            yield return new WaitForSeconds(_waitTime);
            waiting = false;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = null;
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_startLocation, Vector3.one / 2);
            Gizmos.DrawCube(_targetLocation, Vector3.one / 2);
        }
#endif
    }
}
