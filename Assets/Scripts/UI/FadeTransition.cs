using UnityEngine;

namespace Malicious.UI
{
    public class FadeTransition : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int _inParameter = Animator.StringToHash("FadeIn");
        private static readonly int _outParameter = Animator.StringToHash("FadeOut");

        //int for the id of the parameter to set active or not active with fade
        //int for the id of the parameter to set active or not active with fade
        private void Start()
        {
            _animator = GetComponent<Animator>();
            FadeIn();
        }

        [ContextMenu("Fade In")]
        public void FadeIn()
        {
            _animator.SetBool(_inParameter, true);
            _animator.SetBool(_outParameter, false);
        }

        [ContextMenu("Fade Out")]
        public void FadeOut()
        {
            _animator.SetBool(_inParameter, false);
            _animator.SetBool(_outParameter, true);
        }
    }
}