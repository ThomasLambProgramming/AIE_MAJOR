using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.Interactables
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] private LayerMask _launchMask;
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _animationTime = 1f;

        private Animator _launchAnimation = null;
        
        private bool _resetting;
        private static readonly int _Launched = Animator.StringToHash("Launched");


        private void Start()
        {
            _launchAnimation = GetComponent<Animator>();
        }

        private IEnumerator Launched()
        {
            _resetting = true;
            yield return new WaitForSeconds(_animationTime);
            _launchAnimation.SetBool(_Launched, false);
            _resetting = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((_launchMask & (1 << other.gameObject.layer)) > 0)
            {
                Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();
                if (objectRb != null && !_resetting)
                {
                    Vector3 rbVel = objectRb.velocity;
                    rbVel.y = _launchForce;
                    objectRb.velocity = rbVel;
                    _launchAnimation.SetBool(_Launched, true);
                    StartCoroutine(Launched());
                }
            }
        }
    }
}
