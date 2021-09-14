using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Interactables
{
    public class ControlPanel : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _reusable = true;
        [SerializeField] private UnityEvent _onEvent;
        [SerializeField] private UnityEvent _offEvent;

        [SerializeField] private bool _hasHoldOption = false;
        [SerializeField] private float _holdTime = 2f;
        [SerializeField] private UnityEvent _holdEvent;
        
        private bool _beenHacked = false;
        public void OnHackValid()
        { 
            
        }

        public void OnHackFalse()   
        {
            
        }
        public float HackedHoldTime() => _holdTime;
        public bool HasHoldInput() => _hasHoldOption;

        public void HoldInputActivate()
        {
            _holdEvent?.Invoke();
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
            if (!isOn)
            {
                _onEvent?.Invoke();
                isOn = true;
            }
            else
            {
                if (_reusable)
                {
                    _offEvent?.Invoke();
                    isOn = false;
                }
            }
        }
    }
}
