using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Malicious.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _hudObject = null;
        [SerializeField] private GameObject _pauseObject = null;
        [SerializeField] private GameObject _optionsObject = null;
        [SerializeField] private GameObject _controlSchemeObject = null;
        [SerializeField] private GameObject _firstObjectInPause = null;
        [SerializeField] private GameObject _firstObjectInOptions = null;
        
        [SerializeField] private Button _optionsButton = null;
        [SerializeField] private Button _controlLayoutButton = null;

        private EventSystem _eventSystem = null;
        void Start()
        {
            _eventSystem = FindObjectOfType<EventSystem>();
            GameEventManager.GamePauseStart += SetFirstToPause;
            GameEventManager.GamePauseStart += OnPause;
            GameEventManager.GamePauseExit += SetFirstToPause;
            GameEventManager.GamePauseExit += OnPauseExit;

            //_pauseButton = _pauseObject.GetComponent<Button>();
            //_optionsButton = _optionsObject.GetComponent<Button>();
        }
        private void OnPause()
        {
            _pauseObject.SetActive(true);
            _hudObject.SetActive(false);
            _optionsObject.SetActive(false);
            _controlSchemeObject.SetActive(false);
            GlobalData.InputManager.Player.BackButton.performed += BackOut;
            SetFirstToPause();
            
        }

        private void OnPauseExit()
        {
            _hudObject.SetActive(true);
            _pauseObject.SetActive(false);
            _optionsObject.SetActive(false);
            _controlSchemeObject.SetActive(false);
            GlobalData.InputManager.Player.BackButton.performed -= BackOut;
        }

        private void BackOut(InputAction.CallbackContext a_context)
        {
            if (_optionsObject.activeInHierarchy || _controlSchemeObject.activeInHierarchy)
            {
                _optionsObject.SetActive(false);
                _pauseObject.SetActive(true);
                _controlSchemeObject.SetActive(false);
                SetFirstToPause();
            }
            else if (_pauseObject.activeInHierarchy)
            {
                GameEventManager.ResumePlay();
            }
        }
        public void SetFirstInOptions()
        {
            _eventSystem.SetSelectedGameObject(_firstObjectInOptions);
            _optionsButton.Select();
        }
        public void SetFirstToPause()
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_firstObjectInPause);
        }
        public void SetFirstToControlScheme()
        {
            _eventSystem.SetSelectedGameObject(_controlSchemeObject);
            _controlLayoutButton.Select();
        }
    }
}
