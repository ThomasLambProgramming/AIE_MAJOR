using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Hackable;
using UnityEngine;
using Malicious.Core;

namespace Malicious.Interactables
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] private LayerMask _launchMask;
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _blockLaunchForce = 10f;
        [SerializeField] private float _groundEnemyLaunchForce = 10f;
        [SerializeField] private float _horizontalAllowance = 3f;
        [SerializeField] private float _animationTime = 1f;
        [SerializeField] private float _blockHorizontalAllow = 1.5f;
        [SerializeField] private AudioSource _springAudio = null;
        private Animator _launchAnimation = null;

        [SerializeField] Animation _animLaunch = null;

        [ContextMenu("LaunchTest")]
        public void LaunchTest()
        {
            if (_animLaunch != null)
                _animLaunch.Play();
        }
        
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
            _resetting = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;
            
            Vector3 directionToObject = other.gameObject.transform.position - transform.position;
            directionToObject.y = 0;
            
            if (other.gameObject.layer == 16 || other.gameObject.layer == 15)
            {
                //since blocks stick out past their mid point it needs extra allowance then the player
                if (directionToObject.magnitude > _blockHorizontalAllow)
                    return;
            }
            else if (directionToObject.magnitude > _horizontalAllowance)
                return;
            
            if ((_launchMask & (1 << other.gameObject.layer)) > 0)
            {
                Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();
                if (objectRb != null && !_resetting)
                {
                    Vector3 rbVel = objectRb.velocity;

                    if (other.gameObject.layer == 10)
                    {
                        rbVel.y = _launchForce;
                        other.gameObject.GetComponent<Player>().SpringLaunch();
                    }
                    if (other.gameObject.layer == 16 || other.gameObject.layer == 15)
                        rbVel.y = _blockLaunchForce;
                    if (other.gameObject.layer == 11)
                        rbVel.y = _groundEnemyLaunchForce;


                    objectRb.velocity = rbVel;
                    _launchAnimation.SetBool(_Launched, true);
                    _springAudio.Play();
                    StartCoroutine(Launched());
                }
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * _horizontalAllowance);
            Gizmos.DrawLine(transform.position, transform.position + transform.right * _horizontalAllowance);
        }
        #endif
    }
}
