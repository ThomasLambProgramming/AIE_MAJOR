using System;
using System.Collections;
using UnityEngine;

namespace Malicious.GameItems
{
    public class Bridge : MonoBehaviour
    {
        [SerializeField] private GameObject _pathObject = null;
        [SerializeField] private float _scaleGoal = 5f;
        [SerializeField] private float _scaleSpeed = 1f;

        [SerializeField] private GameObject _railObject = null;
        [SerializeField] private float _railHeightTarget = 1f;
        private float _railHeight = 0;
        [SerializeField] private float _railSpeed = 1f;
        private float _startingRailHeight = 0;
        
        
        private float _startingScale = 0;
        private float _timer = 0;
        private float _railTimer = 0;
        
        private void Start()
        {
            _startingScale = _pathObject.transform.localScale.x;
            if (_railObject != null)
                _startingRailHeight = _railObject.transform.position.y;

            _railHeight = _startingRailHeight + _railHeightTarget;
        }

        [ContextMenu("BridgeOn")]
        public void BridgeOn()
        {
            StopCoroutine(TurnBridgeOff());
            StartCoroutine(TurnBridgeOn());
        }

        [ContextMenu("BridgeOff")]
        public void BridgeOff()
        {
            StopCoroutine(TurnBridgeOn());
            StartCoroutine(TurnBridgeOff());
        }

        private IEnumerator TurnBridgeOn()
        {
            
                
            while (_timer < 1)
            {
                _timer += Time.deltaTime * _scaleSpeed;
                Vector3 bridgeScale = _pathObject.transform.localScale;
                bridgeScale.x = Mathf.Lerp(_startingScale, _scaleGoal, _timer);
                _pathObject.transform.localScale = bridgeScale;
                yield return null;
            }
            _timer = 1;

            if (_railObject != null)
            {
                while (_railTimer < 1)
                {
                    _railTimer += Time.deltaTime * _railSpeed;
                    Vector3 railPosition = _railObject.transform.position;
                    railPosition.y = Mathf.Lerp(_startingRailHeight, _railHeight, _railTimer);
                    _railObject.transform.position = railPosition;
                    yield return null;
                }
                _railTimer = 1f;
            }
        }
        private IEnumerator TurnBridgeOff()
        {
            if (_railObject != null)
            {
                while (_railTimer > 0)
                {
                    _railTimer -= Time.deltaTime * _railSpeed;
                    Vector3 railPosition = _railObject.transform.position;
                    railPosition.y = Mathf.Lerp(_startingRailHeight, _railHeight, _railTimer);
                    _railObject.transform.position = railPosition;
                    yield return null;
                }
                _railTimer = 0f;
            }

            
            while (_timer > 0)
            {
                _timer -= Time.deltaTime * _scaleSpeed;
                Vector3 bridgeScale = _pathObject.transform.localScale;
                bridgeScale.x = Mathf.Lerp(_startingScale, _scaleGoal, _timer);
                _pathObject.transform.localScale = bridgeScale;
                yield return null;
            }
            _timer = 0; 
            
        }
    }
}