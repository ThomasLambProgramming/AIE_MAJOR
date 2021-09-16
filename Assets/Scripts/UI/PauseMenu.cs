using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;

namespace Malicious.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _hudObject = null;
        [SerializeField] private GameObject _pauseObject = null;
        [SerializeField] private GameObject _optionsObject = null;
        void Start()
        {
            GameEventManager.GamePauseStart += OnPause;
            GameEventManager.GamePauseExit += OnPauseExit;
        }
        private void OnPause()
        {
            _pauseObject.SetActive(true);
            _hudObject.SetActive(false);
            _optionsObject.SetActive(false);
        }

        private void OnPauseExit()
        {
            _hudObject.SetActive(true);
            _pauseObject.SetActive(false);
            _optionsObject.SetActive(false);
        }
    }
}
