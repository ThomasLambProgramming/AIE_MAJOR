using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Malicious.Hackable
{
    public class WireModelDissolve : MonoBehaviour
    {
        [SerializeField] private float _dissolveSpeed = 1f;
        private MeshRenderer _renderer = null;

        private float _currentDissolveAmount = 0;
        private int _dissolveAmountID = Shader.PropertyToID("Test");

        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _currentDissolveAmount = _renderer.material.GetFloat(_dissolveAmountID);
            DissolveIn();
        }

        [ContextMenu("DissolveIn")]
        public void DissolveIn()
        {
            StopCoroutine(DissolveToVoid(false));
            StartCoroutine(DissolveIntoMaterial());
        }
        [ContextMenu("DissolveOut")]
        public void DissolveOut(bool a_delete)
        {
            StopCoroutine(DissolveIntoMaterial());
            StartCoroutine(DissolveToVoid(a_delete));
        }

        private IEnumerator DissolveIntoMaterial()
        {
            bool doneDissolving = false;
            
            while (doneDissolving == false)
            {
                if (_currentDissolveAmount >= 0)
                {
                    _currentDissolveAmount -= Time.deltaTime * _dissolveSpeed;
                    _renderer.material.SetFloat(_dissolveAmountID, _currentDissolveAmount);
                }
                else
                {
                    doneDissolving = true;
                }
                yield return null;
            }
        }
        private IEnumerator DissolveToVoid(bool a_delete)
        {
            bool doneDissolving = false;
            
            while (doneDissolving == false)
            {
                if (_currentDissolveAmount <= 1)
                {
                    _currentDissolveAmount += Time.deltaTime * _dissolveSpeed;
                    _renderer.material.SetFloat(_dissolveAmountID, _currentDissolveAmount);
                }
                else
                {
                    doneDissolving = true;
                }
                yield return null;
            }
            if (a_delete)
                Destroy(gameObject);
        }
    }
}
