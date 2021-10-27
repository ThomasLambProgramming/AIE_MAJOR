using UnityEngine;

namespace Malicious.UI
{
    public class FadeTransition : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int _inParameter = Animator.StringToHash("FadeIn");
        private static readonly int _outParameter = Animator.StringToHash("FadeOut");
        [SerializeField] private float _fadeInSpeed = 1;
        [SerializeField] private float _fadeOutSpeed = 1;


        //int for the id of the parameter to set active or not active with fade
        //int for the id of the parameter to set active or not active with fade
        private void Start()
        {
            _animator = GetComponent<Animator>();
            FadeIn();

            _animator.SetFloat("FadeOutSpeed", _fadeOutSpeed);
            _animator.SetFloat("FadeInSpeed", _fadeInSpeed);
        }

        [ContextMenu("Fade In")]
        public void FadeIn()
        {
            if (_animator != null)
            {
                _animator.SetBool(_inParameter, true);
                _animator.SetBool(_outParameter, false);
            }
        }

        [ContextMenu("Fade Out")]
        public void FadeOut()
        {
            if (_animator != null)
            {
                _animator.SetBool(_inParameter, false);
                _animator.SetBool(_outParameter, true);
            }
        }
    }
}