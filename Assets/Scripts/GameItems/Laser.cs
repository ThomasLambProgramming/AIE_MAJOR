using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.GameItems
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private float _laserDistance = 50f;
        [SerializeField] private Transform _playerCollider = null;
        [SerializeField] private Vector3 _laserDirection = Vector3.zero;
        [SerializeField] private Vector2 _laserWidth = Vector2.zero;
        [SerializeField] private Transform _laserStartPosition = null;
        [SerializeField] private LayerMask _laserMask = ~0;
        private LineRenderer _lineRenderer = null;

        private bool _isActive = true;
        // Start is called before the first frame update
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>(); 
            if (_laserDirection == Vector3.zero)
            {
                _laserDirection = transform.up;
            }
            else
            {
                _playerCollider.rotation = Quaternion.LookRotation(_laserDirection);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_isActive)
                return;
            
            RaycastHit hit;
            Ray ray = new Ray(_laserStartPosition.position, _laserDirection);
            ray.direction = _laserDirection;
            if (Physics.Raycast(ray,out hit, _laserDistance, _laserMask))
            {
                _playerCollider.position = (hit.point + _laserStartPosition.position) / 2;
                Vector3 newColliderScale = hit.point - _laserStartPosition.position;
                _playerCollider.localScale = new Vector3(_laserWidth.x, newColliderScale.magnitude, _laserWidth.y);
                
                Vector3[] positions = {_laserStartPosition.position, hit.point};
                _lineRenderer.SetPositions(positions);
            }
        }

        public void TurnOff()
        {
            _isActive = false;
            _lineRenderer.enabled = false;
            _playerCollider.gameObject.SetActive(false);
        }

        public void TurnOn()
        {
            _isActive = true;
            _lineRenderer.enabled = true;
            _playerCollider.gameObject.SetActive(true);
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(_laserStartPosition.position, _laserStartPosition.position + _laserDirection * 2);
        }
#endif
    }
}
