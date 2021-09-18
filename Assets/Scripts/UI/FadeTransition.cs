using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        }

        [ContextMenu("Fade In")]
        public void FadeIn()
        {
            _animator.SetTrigger(_inParameter);
        }

        [ContextMenu("Fade Out")]
        public void FadeOut()
        {
            _animator.SetTrigger(_outParameter);
        }
    }
}