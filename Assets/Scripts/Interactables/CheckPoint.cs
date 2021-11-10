using UnityEngine;

namespace Malicious.Interactables
{
    public class CheckPoint : MonoBehaviour
    {
        private Animator _animator = null;
        [Tooltip("SETTING ID IS A REQUIREMENT, ORDER THEM BY APPEARANCE IN THE LEVEL")]
        public int _ID = 0;
        public Vector3 _returnPosition = Vector3.zero;
        public Vector3 _facingDirection = Vector3.zero;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        public void TurnOn()
        {
            _animator.SetBool("Active", true);
        }
        public void TurnOff()
        {
            _animator.SetBool("Active", false);
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