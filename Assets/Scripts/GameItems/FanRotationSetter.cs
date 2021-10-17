using UnityEngine;
using UnityEngine.Events;

namespace Malicious.GameItems
{
    public class FanRotationSetter : MonoBehaviour
    {
        [SerializeField] private Fan _fanToAffect = null;
        [SerializeField] private Vector3 _goalRotation = Vector3.zero;

        public void RotateFan()
        {
            _fanToAffect.RotateFan(_goalRotation);
        }
    }
}