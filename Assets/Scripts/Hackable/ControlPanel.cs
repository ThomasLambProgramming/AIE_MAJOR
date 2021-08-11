using Malicious.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    public class ControlPanel : MonoBehaviour, IHackable
    {
        [SerializeField] private UnityEvent onHackedEvent;

        public void Hacked()
        {
            onHackedEvent?.Invoke();
        }
    }
}
