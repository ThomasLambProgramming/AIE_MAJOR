using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
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
        
        private ParticleSystem _particleSystem = null;
        
        
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
            if (_isLevelChangeCore)
            {
                StartCoroutine(LoadNextLevel());
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
