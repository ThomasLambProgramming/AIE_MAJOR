using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Malicious.UI
{
    public class Subtitles : MonoBehaviour
    {
        public static Subtitles _subtitles = null;
        [SerializeField] private float _fadeSpeed = 2f;
        [SerializeField] private float _waitTime = 5f;

        private static bool _displayingText = false;

        private Text _text = null;
        private Coroutine _fadeOutCoroutine = null;

        private List<string> _speech = new List<string>();
        void Start()
        {
            _text = GetComponent<Text>();
            _subtitles = this;
        }

        public void ShowText(string a_text)
        {
            //This is redundancy to avoid double ups and have the text fade early
            if (_fadeOutCoroutine != null)
                StopCoroutine(_fadeOutCoroutine);

            StopCoroutine(WaitToFadeOut());
            if (_displayingText)
            {
                HideText();
                StartCoroutine(StartText(1.7f, a_text));
            }
            else
            {
                StartCoroutine(StartText(0.1f, a_text));
            }
            
        }
        
        public void HideText()
        {
            StartCoroutine(FadeOut());
        }
        IEnumerator StartText(float a_waitDurection, string a_text)
        {
            yield return new WaitForSeconds(a_waitDurection);
            _displayingText = true;
            _text.text = a_text;
            StartCoroutine(FadeIn());
        }
        IEnumerator WaitToFadeOut()
        {
            yield return new WaitForSeconds(_waitTime);
            StartCoroutine(FadeOut());
        }
        IEnumerator FadeIn()
        {
            Color textColor = _text.color;
            
            while (textColor.a < 1)
            {
                textColor.a += _fadeSpeed * Time.deltaTime;
                _text.color = textColor;
                yield return null;
            }
            textColor.a = 1;
            _text.color = textColor;
            _fadeOutCoroutine = StartCoroutine(WaitToFadeOut());
        }
        IEnumerator FadeOut()
        {
            Color textColor = _text.color;

            while (textColor.a > 0)
            {
                textColor.a -= _fadeSpeed * Time.deltaTime;
                _text.color = textColor;
                yield return null;
            }
            textColor.a = 0;
            _text.color = textColor;
            _displayingText = false;
            _fadeOutCoroutine = null;
        }
        public void LevelCoreStop()
        {
            _fadeSpeed = 6f;
            HideText();
        }
    }
}
