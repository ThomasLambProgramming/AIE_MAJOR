using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Tools
{
    public class EventTester : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event = null;

        [ContextMenu("RunEvent")]
        public void RunEvent()
        {
            _event?.Invoke();
        }
    }
}