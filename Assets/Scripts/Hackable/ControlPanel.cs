using Malicious.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Malicious.Player;

namespace Malicious.Hackable
{
    public class ControlPanel : MonoBehaviour, IHackable
    {
        [SerializeField] private bool reusable = true;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private UnityEvent offEvent;
        
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
        public void PlayerExit(){}
        public HackableInformation GiveInformation() =>
            new HackableInformation(gameObject, null, null, ObjectType.ControlPanel);

        private void OnTriggerEnter(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                Malicious.Player.PlayerController.PlayerControl.SetInteractable(this);
            }
        }

        private void OnTriggerExit(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                Malicious.Player.PlayerController.PlayerControl.SetInteractable(null);
            }
        }
    }
}
