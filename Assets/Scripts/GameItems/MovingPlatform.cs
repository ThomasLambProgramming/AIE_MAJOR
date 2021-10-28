using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float _horizontalAllowance = 3f;
        private float _sqrHorizontalAllowance = 9f;
        private GameObject _playerObject = null;

        [SerializeField] private GameObject _middlePosition = null;
        [SerializeField] private float _checkDistance = 4;
        [SerializeField] private LayerMask _stopMask = ~0;

        private float _timer = 0;
        private bool waiting = false;
        private bool _moveToTarget = true;

        private List<GameObject> _attachedObjects = new List<GameObject>();
        
        private Vector3 _sceneStartLocation = Vector3.zero;
        
        void Start()
        {
            GameEventManager.PlayerDead += ResetToStart;
            _startLocation = transform.position;
            _sceneStartLocation = transform.position;
            
            if (_waitPlayer)
                waiting = true;

            _sqrHorizontalAllowance = _horizontalAllowance * _horizontalAllowance;
        }

        [ContextMenu("SetPositions")]
        public void MakePositions()
        {
            _startLocation = transform.position;
            _targetLocation = transform.position;
        }
        
        
        void FixedUpdate()
        {
            for(int i = 0; i <_attachedObjects.Count; i++)
            {
                Vector3 directionTo = _attachedObjects[i].transform.position - transform.position;
                directionTo.y = 0;
                if (directionTo.sqrMagnitude > _sqrHorizontalAllowance)
                {
                    _attachedObjects[i].transform.parent = null;
                    _attachedObjects.RemoveAt(i);
                    i--;
                }
            }

            //if (Physics.Raycast(_middlePosition.transform.position, (_targetLocation - transform.position).normalized,
            //    _checkDistance))
            //    return;
            
            if (_playerObject != null)
            {
                if (Vector3.SqrMagnitude(_playerObject.transform.position - transform.position) > 5)
                {
                    _playerObject.transform.parent = null;
                    _playerObject = null;
                }
            }
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
            if (other.isTrigger)
                return;
            
            if (other.gameObject.CompareTag("Player") || 
                other.gameObject.CompareTag("Hackable") || 
                other.gameObject.CompareTag("Enemy") || 
                other.gameObject.CompareTag("Interactable") || 
                other.gameObject.CompareTag("Block")|| 
                other.gameObject.CompareTag("FlyingEnemy"))
            {
                _playerObject = other.gameObject;
                other.transform.parent = this.transform;
                if (_waitPlayer && waiting)
                {
                    waiting = false;
                }
                _attachedObjects.Add(other.gameObject);
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
            if (other.isTrigger)
                return;
            
            if (other.gameObject.CompareTag("Player") || 
                other.gameObject.CompareTag("Hackable") || 
                other.gameObject.CompareTag("Enemy") || 
                other.gameObject.CompareTag("Interactable") || 
                other.gameObject.CompareTag("Block")|| 
                other.gameObject.CompareTag("FlyingEnemy"))
            {
                other.transform.parent = null;
                _playerObject = null;
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
