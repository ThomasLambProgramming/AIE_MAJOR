using System.Collections;
using UnityEngine;

namespace Malicious.GameItems
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private float _stoppingDistance = 0.4f;
        
        [SerializeField] private Vector3 _targetLocation = Vector3.zero;
        private Vector3 _startLocation = Vector3.zero;
        Vector3 _movementAmount = Vector3.zero;
        
        [SerializeField] private bool _waitPlayer = false;
        
        [SerializeField] private bool _waitForTime = false;
        [SerializeField] private float _waitTime = 3f;
        private bool waiting = false;
        void Start()
        {
            _startLocation = transform.position;
            _movementAmount = _targetLocation - _startLocation;
            if (_waitPlayer)
                waiting = true;
        }

        IEnumerator WaitAtPoint()
        {
            yield return new WaitForSeconds(_waitTime);
            SwapTarget();
            waiting = false;
        }
        void FixedUpdate()
        {
            if (waiting)
                return;
            //later add a timer for waiting at the position for a short time and a slow down as it gets closer to the platform
            if (Vector3.SqrMagnitude(_targetLocation - transform.position) < _stoppingDistance)
            {
                if (_waitForTime)
                {
                    StartCoroutine(WaitAtPoint());
                    waiting = true;
                }
                else if (_waitPlayer)
                {
                    waiting = true;
                }
                else
                    SwapTarget();
            }
            else
            {
                transform.position += _movementAmount * (Time.deltaTime * _moveSpeed);
            }
        }

        private void SwapTarget()
        {
            Vector3 buffer = _startLocation;
            _startLocation = _targetLocation;
            _targetLocation = buffer;
            _movementAmount = _targetLocation - _startLocation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = this.transform;
                if (_waitPlayer && waiting)
                {
                    SwapTarget();
                    waiting = false;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = null;
            }
        }
    }
}
