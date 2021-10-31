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

        private float _timer = 0;
        private bool waiting = false;
        private bool _moveToTarget = true;

        private List<GameObject> _attachedObjects = new List<GameObject>();
        
        private Vector3 _sceneStartLocation = Vector3.zero;

        private List<Rigidbody> _stoppingObjects = new List<Rigidbody>();
        private bool _stopMoving = false;
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

            if (_stopMoving)
            {
                Debug.Log("stop moving");
                return;
            }
            if (waiting)
            {
                return;
            }

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
            if (other.isTrigger || other.gameObject.CompareTag("Environment"))
                return;
            
            Vector3 posDifference = other.transform.position - transform.position;
            float posYDifference = posDifference.y;
            if (posDifference.sqrMagnitude > _sqrHorizontalAllowance || posYDifference < 0)
            {
                _stoppingObjects.Add(other.attachedRigidbody);
                _stopMoving = true;
                return;
            }
            
            
            if (other.gameObject.CompareTag("Player") || 
                other.gameObject.CompareTag("Hackable") || 
                other.gameObject.CompareTag("Enemy") || 
                other.gameObject.CompareTag("Interactable") || 
                other.gameObject.CompareTag("Block")|| 
                other.gameObject.CompareTag("FlyingEnemy"))
            {
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

            if (_stoppingObjects.Contains(other.attachedRigidbody))
            {
                _stoppingObjects.Remove(other.attachedRigidbody);
                
                if (_stoppingObjects.Count <= 0)
                    _stopMoving = false;
                
                return;
            }
            
            if (other.gameObject.CompareTag("Player") || 
                other.gameObject.CompareTag("Hackable") || 
                other.gameObject.CompareTag("Enemy") || 
                other.gameObject.CompareTag("Interactable") || 
                other.gameObject.CompareTag("Block")|| 
                other.gameObject.CompareTag("FlyingEnemy"))
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
