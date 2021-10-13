using System;
using UnityEngine;

namespace Malicious.GameItems
{
    public class BrokenWire : MonoBehaviour
    {
        [SerializeField] private BoxCollider _areaCollider = null;
        [SerializeField] private float _activeTime = 2f;
        [SerializeField] private float _offTime = 1f;

        [SerializeField] private ParticleSystem _leftSide = null;
        [SerializeField] private ParticleSystem _rightSide = null;

        [SerializeField] private Transform _leftSidePosition = null;
        [SerializeField] private Transform _rightSidePosition = null;
        
        [Tooltip("For electric arcs with line renderer")]
        [SerializeField] private float _minYHeight = 0;
        [SerializeField] private float _maxYHeight = 5f;
        
        private LineRenderer _lineRenderer = null;
        
        private bool _isActive = false;
        [SerializeField] private bool _turnedOn = true;
        private float _timer = 0;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Update()
        {
            if (!_turnedOn)
                return;

            _timer += Time.deltaTime;
            
            if (_isActive)
            {
                if (_timer >= _activeTime)
                {
                    _timer = 0;
                    _isActive = false;
                    _areaCollider.enabled = false;
                }
            }
            else
            {
                if (_timer >= _offTime)
                {
                    _timer = 0;
                    _isActive = true;
                    _areaCollider.enabled = true;
                }
            }
        }

        public void TurnOff()
        {
            _areaCollider.enabled = false;
            _timer = 0;
            _leftSide.Stop();
            _rightSide.Stop();
        }

        public void TurnOn()
        {
            _areaCollider.enabled = true;
            _timer = 0;
            _leftSide.Play();
            _rightSide.Play();
        }
    }
}