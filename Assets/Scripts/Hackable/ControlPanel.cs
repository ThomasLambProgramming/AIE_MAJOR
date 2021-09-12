using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] private bool reusable = true;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private UnityEvent offEvent;
        
        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        private bool _beenHacked = false;
        public void OnHackValid()
        { 
            if (!_beenHacked)
                _nodeRenderer.material = _hackValidMaterial;
        }

        public void OnHackFalse()   
        {
            if (!_beenHacked || reusable)
                _nodeRenderer.material = _defaultMaterial;
        }
        private void Start()
        {
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
        }
        
        private bool isOn = false;
        
#if UNITY_EDITOR
        [SerializeField] private bool testEvents = false;
        private void Update()
        {
            if (testEvents)
            {
                Hacked();
                testEvents = false;
            }
        }
#endif
        
        
        public void Hacked()
        {
            _beenHacked = true;
            _nodeRenderer.material = _hackedMaterial;
            if (!isOn)
            {
                onEvent?.Invoke();
                isOn = true;
            }
            else
            {
                if (reusable)
                {
                    offEvent?.Invoke();
                    isOn = false;
                }
            }
        }
    }
}
