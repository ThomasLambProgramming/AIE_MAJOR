using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Hackable;
using UnityEngine;

namespace Malicious.Interactables
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] private LayerMask _launchMask;
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _blockLaunchForce = 10f;
        [SerializeField] private float _groundEnemyLaunchForce = 10f;

        [SerializeField] private float _animationTime = 1f;
        [SerializeField] private float _dotCheck = 0.6f;
        private Animator _launchAnimation = null;
        
        
        private bool _resetting;
        private static readonly int _Launched = Animator.StringToHash("Launched");


        private void Start()
        {
            _launchAnimation = GetComponentInChildren<Animator>();
        }

        private IEnumerator Launched()
        {
            _resetting = true;
            yield return new WaitForSeconds(_animationTime);
            _launchAnimation.SetBool(_Launched, false);
            _launchAnimation.enabled = false;
            _resetting = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;
            
            Vector3 directionToObject = other.gameObject.transform.position - transform.position;
            directionToObject = directionToObject.normalized;
            
            if (Vector3.Dot(directionToObject, Vector3.up) < _dotCheck)
                return;
            
            if ((_launchMask & (1 << other.gameObject.layer)) > 0)
            {
                Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();
                if (objectRb != null && !_resetting)
                {
                    Vector3 rbVel = objectRb.velocity;

                    if (other.gameObject.layer == 10)
                        rbVel.y = _launchForce;
                    if (other.gameObject.layer == 16)
                        rbVel.y = _blockLaunchForce;
                    if (other.gameObject.layer == 11)
                        rbVel.y = _groundEnemyLaunchForce;


                    objectRb.velocity = rbVel;
                    _launchAnimation.enabled = true;
                    _launchAnimation.SetBool(_Launched, true);
                    StartCoroutine(Launched());
                }
            }
        }
    }
}
