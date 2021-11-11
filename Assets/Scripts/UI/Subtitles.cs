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
        [SerializeField] private float _inbetweenTime = 1.1f;
        private Text _text = null;
        
       
        [ContextMenu("TestingFunction")]
        public void LoadText()
        {
            
        }

        [SerializeField] private List<string> _speech = new List<string>();
        void Start()
        {
            _text = GetComponent<Text>();
            _subtitles = this;
        }

        
        public void GiveText(List<string> a_text, float a_speed)
        {
            _waitTime = a_speed;
            _speech.Clear();
            _speech = a_text;
            _text.text = _speech[0];
            _speech.RemoveAt(0);
            _done = false;
            _fadeIn = true;
        }

        private bool _done = false;
        private bool _fadeIn = false;
        private bool _fadeOut = false;
        private bool _doneFading = true;
        private float _timer = 0;
        private float _waitTimer = 0;
        public void Update()
        {
            if (_doneFading)
            {
                _waitTimer += Time.deltaTime;
                if (_fadeIn)
                {
                    if (_waitTimer > _inbetweenTime)
                    {
                        _waitTimer = 0;
                        _doneFading = false;
                    }
                }
                else if (_waitTimer > _waitTime)
                {
                    _waitTimer = 0;
                    _doneFading = false;
                }
            }
            else if (_fadeIn || _fadeOut)
                Fading();
        }
        private void Fading()
        {
            if (_done)
                return;

            if (_fadeIn)
            {
                _timer += Time.deltaTime * _fadeSpeed;
                Color textColor = _text.color;
                textColor.a = _timer;
                if (_timer >= 1)
                {
                    _fadeIn = false;
                    _timer = 1;
                    textColor.a = 1;
                    _doneFading = true;
                    _fadeOut = true;
                }
                _text.color = textColor;
            }
            else if (_fadeOut)
            {
                _timer -= Time.deltaTime * _fadeSpeed;
                Color textColor = _text.color;
                textColor.a = _timer;
                if (_timer <= 0)
                {
                    _fadeIn = false;
                    _timer = 0;
                    textColor.a = 0;
                    _fadeIn = true;
                    _doneFading = true;
                    
                    if (_speech.Count > 0)
                    {
                        _text.text = _speech[0];
                        _speech.RemoveAt(0);
                    }
                    else
                    {
                        _done = true;
                    }

                }
                _text.color = textColor;
            }
        }
        
        public void LevelCoreStop()
        {
            _fadeSpeed = 6f;
            _done = false;
            _doneFading = false;
            _fadeOut = true;
        }
    }
}
