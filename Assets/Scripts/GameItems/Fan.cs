using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Hackable;
using UnityEngine;

namespace Malicious.GameItems
{
    [SelectionBase]
    public class Fan : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;

        [SerializeField] private GameObject _rotateObject = null;
        [SerializeField] private float _rotateFanSpeed = 10f;
        [SerializeField] private float _rotateSpeed = 10f;
    
        [SerializeField] private float _fanHeight = 5f;
        [SerializeField] private float _horizontalDifferenceAllow = 2f;
        private float _sqrhorizontalDifferenceAllow = 2f;


        [SerializeField] private LayerMask _objectsAllowed = ~0;
        [SerializeField] private Vector3 _launchDirection = Vector3.up;

        [SerializeField] private float _minPlayerForce = 3f;
        [SerializeField] private float _minBlockForce = 3f;
        [SerializeField] private float _minGroundEnemyForce = 3f;
        [SerializeField] private float _minFlyingEnemyForce = 3f;
        [SerializeField] private float _minSpringForce = 3f;

        [SerializeField] private float _maxPlayerForce = 10f;
        [SerializeField] private float _maxBlockForce = 10f;
        [SerializeField] private float _maxGroundEnemyForce = 10f;
        [SerializeField] private float _maxFlyingEnemyForce = 10f;
        [SerializeField] private float _maxSpringForce = 10f;

        private List<Rigidbody> _playerList = new List<Rigidbody>();
        private List<Rigidbody> _blockList = new List<Rigidbody>();
        private List<Rigidbody> _groundEnemyList = new List<Rigidbody>();
        private List<Rigidbody> _flyingEnemyList = new List<Rigidbody>();
        private List<Rigidbody> _springList = new List<Rigidbody>();

        private void Start()
        {
            _sqrhorizontalDifferenceAllow = _horizontalDifferenceAllow * _horizontalDifferenceAllow;
            if (_launchDirection == Vector3.zero)
                _launchDirection = transform.up;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return; 
            
            Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();

            if (objectRb != null && (_objectsAllowed & (1 << other.gameObject.layer)) > 0)
            {
                    Debug.Log(other.gameObject.layer);
                switch(other.gameObject.layer)
                {
                    case 10:
                        _playerList.Add(objectRb);
                        break;
                    case 11:
                        _groundEnemyList.Add(objectRb);
                        break;
                    case 14:
                        _flyingEnemyList.Add(objectRb);
                        break;
                    //case 10:
                    //    _playerList.Add(objectRb);
                    //case 10:
                    //    _playerList.Add(objectRb);
                    //    break;
                }
            }


          
        }

        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            ApplyForces(ref _playerList, _minPlayerForce, _maxPlayerForce);
            ApplyForces(ref _groundEnemyList, _minGroundEnemyForce, _maxGroundEnemyForce);
            ApplyForces(ref _flyingEnemyList, _minFlyingEnemyForce, _maxFlyingEnemyForce);


        }
        private void ApplyForces(ref List<Rigidbody> a_list, float a_minForce, float a_maxForce)
        {
            for (int i = 0; i < a_list.Count; i++)
            {
                Vector3 difference = a_list[i].transform.position - transform.position;
                Vector2 horizontalDiff = new Vector2(difference.x, difference.z);

                if (horizontalDiff.sqrMagnitude > _sqrhorizontalDifferenceAllow || difference.y > _fanHeight)
                {
                    a_list.RemoveAt(i);
                    if (a_list.Count > 0)
                        i--;

                    continue;
                }

                float forceScale = Mathf.Lerp(a_minForce, a_maxForce, difference.y / _fanHeight);

                Vector3 forceToApply = _launchDirection * forceScale;
                a_list[i].AddForce(forceToApply);
            }
        }
        public void RotateFan(Vector3 a_goalRotation)
        {
            //insurance that its not overlapping rotates
            StopCoroutine(RotateFanEnumerator(Vector3.zero));
            StartCoroutine(RotateFanEnumerator(a_goalRotation));
        }
        private IEnumerator RotateFanEnumerator(Vector3 a_target)
        {
            Quaternion goalRot = Quaternion.Euler(a_target);

            while (transform.rotation != goalRot)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    goalRot, 
                    _rotateFanSpeed * Time.deltaTime);
                _launchDirection = -transform.up;
                yield return null;
            }
        }

        private void Update()
        {
            if (_isActive)
                _rotateObject.transform.Rotate(0,_rotateSpeed * Time.deltaTime,0);  
        }

        public void Deactivate() => _isActive = false;
        public void Activate() => _isActive = true;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 currentPosition = transform.position;
            currentPosition.y += _fanHeight;
            Gizmos.DrawLine(transform.position, currentPosition);
            Gizmos.DrawLine(transform.position, transform.position + _launchDirection * 4);
        }
#endif
    }
}
