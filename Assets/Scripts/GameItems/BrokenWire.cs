using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Malicious.GameItems
{
    public class BrokenWire : MonoBehaviour
    {
        [SerializeField] private BoxCollider _areaCollider = null;
        [SerializeField] private float _activeTime = 2f;
        [SerializeField] private float _offTime = 1f;

        [SerializeField] private float _arcWidth = 0.5f;
        [SerializeField] private float _arcOnTime = 1;
        [SerializeField] private float _arcOffTime = 1;
        [SerializeField] private int _minArcs = 1;
        [SerializeField] private int _maxArcs = 5;

        [SerializeField] private ParticleSystem _leftSide = null;
        [SerializeField] private ParticleSystem _rightSide = null;

        [SerializeField] private Transform _leftSidePosition = null;
        [SerializeField] private Transform _rightSidePosition = null;

        [Tooltip("For electric arcs with line renderer")]
        [SerializeField] private float _minYHeight = 0;
        [SerializeField] private float _maxYHeight = 5f;
        
        private LineRenderer _lineRenderer = null;
        
        [SerializeField] private bool _turnedOn = true;
        private bool _isActive = false;
        private bool _isArcActive = false;
        
        private float _Activetimer = 0;
        private float _arcTimer = 0;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.startWidth = _arcWidth;
            _lineRenderer.endWidth = _arcWidth;
        }

        public void Update()
        {
            if (!_turnedOn)
                return;

            _Activetimer += Time.deltaTime;
            
            if (_isActive)
            {
                if (_Activetimer >= _activeTime)
                {
                    _Activetimer = 0;
                    _isActive = false;
                    _arcTimer = 0;
                    _areaCollider.enabled = false;
                    _lineRenderer.enabled = false;
                }
            }
            else
            {
                if (_Activetimer >= _offTime)
                {
                    _Activetimer = 0;
                    _isActive = true;
                    _arcTimer = 0;
                    _lineRenderer.enabled = true;
                    _areaCollider.enabled = true;
                }
            }

            
            if (_lineRenderer.enabled)
            {
                _arcTimer += Time.deltaTime;

                if (_isArcActive && _arcTimer >= _arcOnTime)
                {
                    _arcTimer = 0;
                    _lineRenderer.SetPositions(null);
                }
                if (_arcTimer >= _arcOffTime)
                {
                    _arcTimer = 0;

                    int arcs = Random.Range(_minArcs, _maxArcs);
                    
                    Vector3[] arcPositions = new Vector3[arcs + 2];
                    _lineRenderer.positionCount = arcPositions.Length;
                    arcPositions[0] = _leftSidePosition.position;
                    arcPositions[arcPositions.Length - 1] = _rightSidePosition.position;

                    Vector3 positionDifference = _rightSidePosition.position - _leftSidePosition.position;
                    
                    Vector3 directionToArc = positionDifference.normalized;
                    
                    float length = positionDifference.magnitude;
                    positionDifference.y = 0;

                    float averageDistance = length / arcs;
                    float randomDistanceToAdd = averageDistance * 0.3f;
                    
                    for (int i = 1; i < arcPositions.Length - 1; i++)
                    {
                        if (i == arcPositions.Length - 2)
                        {
                            arcPositions[i] = _leftSidePosition.position +
                                              directionToArc * (averageDistance * i)
                                              + (Random.Range(-randomDistanceToAdd, 0) *
                                                 directionToArc);
                        }
                        else
                        {
                            arcPositions[i] = _leftSidePosition.position +
                                              directionToArc * (averageDistance * i)
                                              + (Random.Range(-randomDistanceToAdd, randomDistanceToAdd) *
                                                 directionToArc);
                        }
                        arcPositions[i].y = Random.Range(_minYHeight, _maxYHeight);
                    }
                    _lineRenderer.SetPositions(arcPositions);
                }
            }
        }

        public void TurnOff()
        {
            _areaCollider.enabled = false;
            _lineRenderer.enabled = true;
            _Activetimer = 0;
            _leftSide.Stop();
            _rightSide.Stop();
        }

        public void TurnOn()
        {
            _lineRenderer.enabled = false;
            _areaCollider.enabled = true;
            _Activetimer = 0;
            _leftSide.Play();
            _rightSide.Play();
        }
    }
}