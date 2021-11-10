using UnityEngine;
using System;
using System.Collections;

namespace Malicious.Interactables
{
    public class CheckPoint : MonoBehaviour
    {
        private Animator _animator = null;
        [Tooltip("SETTING ID IS A REQUIREMENT, ORDER THEM BY APPEARANCE IN THE LEVEL")]
        public int _ID = 0;
        public Vector3 _returnPosition = Vector3.zero;
        public Vector3 _facingDirection = Vector3.zero;

        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        [SerializeField] private Material _baseHacked = null;
        [SerializeField] private Material _baseDefault = null;

        [SerializeField] private MeshRenderer _mainBeacon = null;
        [SerializeField] private MeshRenderer _beaconTopRenderer = null;
        [SerializeField] private MeshRenderer _middleBeaconRenderer = null;

        [SerializeField] private GameObject _beaconObj = null;
        private float _openHeight = 2.3f;
        private float _closeHeight = 1.76f;

        private float _timer = 0;
        private float _animSpeed = 2f;
        private void Start()
        {
            _animator = GetComponent<Animator>();

            _openHeight = _openHeight + transform.position.y;
            _closeHeight = _closeHeight + transform.position.y;
        }
        public void TurnOn()
        {
            StartCoroutine(Open());
            _beaconTopRenderer.material = _hackedMaterial;
            _middleBeaconRenderer.material = _hackedMaterial;
            _mainBeacon.material = _baseHacked;
        }
        public void TurnOff()
        {
            StartCoroutine(Close());
            _beaconTopRenderer.material = _defaultMaterial;
            _middleBeaconRenderer.material = _defaultMaterial;
            _mainBeacon.material = _baseDefault;
        }
        IEnumerator Open()
        {
            while (_beaconObj.transform.position.y != _openHeight)
            {
                _timer += Time.deltaTime * _animSpeed;
                float y = Mathf.Lerp(_closeHeight, _openHeight, _timer);
                Vector3 position = _beaconObj.transform.position;
                position.y = y;
                _beaconObj.transform.position = position;

                if (_timer > 1)
                {
                    _timer = 1;
                }
                yield return null;
            }
        }
        IEnumerator Close()
        {
            while (_beaconObj.transform.position.y != _closeHeight)
            {
                _timer -= Time.deltaTime * _animSpeed;
                float y = Mathf.Lerp(_closeHeight, _openHeight, _timer);
                Vector3 position = _beaconObj.transform.position;
                position.y = y;
                _beaconObj.transform.position = position;

                if (_timer < 0)
                {
                    _timer = 0;
                }
                yield return null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_returnPosition, Vector3.one);
            Gizmos.DrawLine(new Vector3(
                _returnPosition.x, _returnPosition.y + 2, _returnPosition.z), 
                _returnPosition + _facingDirection);
        }
        #endif
    }
}