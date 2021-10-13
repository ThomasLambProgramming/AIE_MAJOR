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
        [SerializeField] private float _animationTime = 2f;

        private Animator _animator = null;
        
        private bool _resetting;


        private void Start()
        {
            //_animator = GetComponent<Animator>()
        }

        private IEnumerator Launched()
        {
            _resetting = true;

            yield return new WaitForSeconds(_animationTime);
            
            //_animator.SetBool("Launched", false);
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
                    //_animator.SetBool("Launched", true);
                    StartCoroutine(Launched());
                }
            }
        }
    }
}
