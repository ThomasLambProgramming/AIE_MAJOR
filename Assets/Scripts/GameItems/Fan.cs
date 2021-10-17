using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.GameItems
{
    [SelectionBase]
    public class Fan : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;
        [SerializeField] private GameObject _rotateObject = null;
        [SerializeField] private float _rotateFanSpeed = 10f;
        [SerializeField] private Vector3 _launchDirection = Vector3.up;
        [SerializeField] private float _velocityLimitAtPeak = 4f;
        [SerializeField] private float _velocityLimitAtBottom = 10f;
        [Tooltip("I dont know why im keeping the two propel forces but im too tired to fix im scared to break")]
        [SerializeField] private float _maxPropelForce = 30f;
        [SerializeField] private float _minPropelForce = 9.83f;
        [SerializeField] private float _fanHeight = 5f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private LayerMask _objectsAllowed = ~0;
        [SerializeField] private List<Rigidbody> _activeObjects = new List<Rigidbody>();

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();
            if (objectRb != null && (_objectsAllowed & (1 << other.gameObject.layer)) > 0)
                _activeObjects.Add(objectRb);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_isActive)
                return;
            
            foreach (var rigidbody in _activeObjects)
            {
                //um so scale force applied by height then percentage of velocity to take away on height as well so fast
                //acc at lower but more hover on end of height
                //float yDifference = Mathf.Abs(rigidbody.gameObject.transform.position.y - transform.position.y) / _fanHeight;

                float posDifference = Mathf.Abs((rigidbody.gameObject.transform.position - transform.position).magnitude / _fanHeight);
                
                float maxVelocity = Mathf.Lerp(_velocityLimitAtBottom, _velocityLimitAtPeak, posDifference);
                float maxSqrVelocity = maxVelocity * maxVelocity;
                
                float forceToApply = Mathf.Lerp(_maxPropelForce, _minPropelForce, posDifference);
                
                if (_launchDirection == Vector3.up)
                {
                    float currentYVel = rigidbody.velocity.y;
                    if (currentYVel > maxVelocity)
                        rigidbody.AddForce(0, maxVelocity, 0);
                    else
                        rigidbody.AddForce(0, forceToApply, 0);
                }
                else
                {
                    Vector3 force = forceToApply * _launchDirection;
                    Vector3 currentVelocity = rigidbody.velocity;
                    if (currentVelocity.sqrMagnitude > maxSqrVelocity)
                    {
                        currentVelocity = currentVelocity.normalized * maxVelocity;
                        rigidbody.velocity = currentVelocity;
                    }
                    else
                    {
                        rigidbody.AddForce(force);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (var VARIABLE in _activeObjects)
            {
                if (VARIABLE.gameObject == other.gameObject)
                {
                    //since trigger is done per object we can exit out with break as the one has been found
                    _activeObjects.Remove(VARIABLE);
                    break;
                }
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
