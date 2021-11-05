using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using Malicious.GameItems;
using Malicious.Interactables;
using Malicious.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    [RequireComponent(typeof(SceneManage), typeof(HackableField))]
    public class LevelCore : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _hackedEvent = null;
        [SerializeField] private bool _isLevelChangeCore;
        [SerializeField] private int _buildIndexOfChange = 0;
        private bool _activated = false;
        private ParticleSystem _particleSystem = null;

        [SerializeField] private List<WallLight> _lights = new List<WallLight>();
        [SerializeField] private Door _door1 = null;
        [SerializeField] private Door _door2 = null;

        // Start is called before the first frame update
        void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void OnHackValid()
        {
        }
        public void OnHackFalse()
        {
        }
        public float HackedHoldTime() => 0;
        public bool HasHoldInput() => false;
        public void HoldInputActivate(){}

        public void Hacked()
        {
            if (_activated)
                return;

            _activated = true;
            if (_isLevelChangeCore)
            {
                StartCoroutine(LoadNextLevel());
            }
            else
            {
                foreach (var wallLight in _lights)
                {
                    wallLight.ChangeToRed();
                }

                if (_door1 != null)
                {
                    _door1.CoreHacked();
                }
                if (_door2 != null)
                {
                    _door2.CoreHacked();
                }
            }
            _hackedEvent?.Invoke();
            
        }

        private IEnumerator LoadNextLevel()
        {
            GameEventManager._fadeTransition.FadeOut();
            yield return new WaitForSeconds(2);
            GameEventManager.Reset();
            GetComponent<SceneManage>().LoadLevelOne(_buildIndexOfChange);
        }
    }
}
