using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Malicious.UI
{
    public class OptionMenu : MonoBehaviour
    {
        private const string _InvertCameraXKey = "CamInvertX";
        private const string _InvertCameraYKey = "CamInvertY";
        private const string _CameraSpeedXKey = "CamSpeedX";
        private const string _CameraSpeedYKey = "CamSpeedY";
        
        public static bool _invertCameraX = false;
        public static bool _invertCameraY = false;
        public static float _cameraSpeedX = 150f;
        public static float _cameraSpeedY = 150f;

        [SerializeField] private bool _defaultInvertX = false;
        [SerializeField] private bool _defaultInvertY = false;
        [SerializeField] private float _defaultCamXSpeed = 150f;
        [SerializeField] private float _defaultCamYSpeed = 4f;
        
        [SerializeField] private GameObject _invertCameraXIndicator = null;        
        [SerializeField] private GameObject _invertCameraYIndicator = null;

        [SerializeField] private Slider _cameraSpeedXSlider = null;
        [SerializeField] private Slider _cameraSpeedYSlider = null;
        
        private void OnEnable()
        {
            if (_invertCameraX)
                _invertCameraXIndicator.SetActive(true);
            else
                _invertCameraXIndicator.SetActive(false);

            if (_invertCameraY)
                _invertCameraYIndicator.SetActive(true);
            else 
                _invertCameraYIndicator.SetActive(false);
        }

        //Running on awake so the camera controller can get the values without needing to get references
        void Awake()
        {
            //check if the values are in the playerprefs
            //if they arent in the key then set to the defaults
            //by the end of this the player prefs will be made.
            if (PlayerPrefs.HasKey(_InvertCameraXKey))
            {
                int holder = PlayerPrefs.GetInt(_InvertCameraXKey);
                if (holder == 0)
                {
                    _invertCameraX = false;
                    _invertCameraXIndicator.SetActive(false);
                }
                else
                {
                    _invertCameraX = true;
                    _invertCameraXIndicator.SetActive(true);   
                }
            }
            else
            {
                _invertCameraX = _defaultInvertX;
                if (_defaultInvertX == false)
                    PlayerPrefs.SetInt(_InvertCameraXKey, 0);
                else
                    PlayerPrefs.SetInt(_InvertCameraXKey, 1);    
            }
                
            if (PlayerPrefs.HasKey(_InvertCameraYKey))
            {
                int holder = PlayerPrefs.GetInt(_InvertCameraYKey);
                if (holder == 0)
                {
                    _invertCameraX = false;
                    _invertCameraXIndicator.SetActive(false);
                }
                else
                {
                    _invertCameraX = true;
                    _invertCameraXIndicator.SetActive(true);
                }
            }
            else
            {
                _invertCameraY = _defaultInvertY;
                if (_defaultInvertY == false)
                    PlayerPrefs.SetInt(_InvertCameraYKey, 0);
                else
                    PlayerPrefs.SetInt(_InvertCameraYKey, 1);    
            }
            
            if (PlayerPrefs.HasKey(_CameraSpeedXKey))
            {
                _cameraSpeedX = PlayerPrefs.GetFloat(_CameraSpeedXKey);
            }
            else
                PlayerPrefs.SetFloat(_CameraSpeedXKey, _defaultCamXSpeed);
                
            
            if (PlayerPrefs.HasKey(_CameraSpeedYKey))
            {
                _cameraSpeedY = PlayerPrefs.GetFloat(_CameraSpeedYKey);
            }
            else
                PlayerPrefs.SetFloat(_CameraSpeedYKey, _defaultCamYSpeed);
                
            //Now we have all the correct values for everything 
            //update all ui values
            if (_invertCameraY)
                _invertCameraXIndicator.SetActive(true);
            if (_invertCameraY)
                _invertCameraYIndicator.SetActive(true);

            _cameraSpeedXSlider.value = _cameraSpeedX;
            _cameraSpeedYSlider.value = _cameraSpeedY;

            _cameraSpeedXSlider.onValueChanged.AddListener(delegate {UpdateSliderX();});
            _cameraSpeedYSlider.onValueChanged.AddListener(delegate {UpdateSliderY();});
            
            //update camera x and y
            //plus inverting
        }

        private void UpdateSliderX()
        {
            _cameraSpeedX = _cameraSpeedXSlider.value;
            PlayerPrefs.SetFloat(_CameraSpeedXKey, _cameraSpeedX);
            CameraController.UpdateCameraX(_cameraSpeedX);
            SaveValue();
        }

        private void UpdateSliderY()
        {
            _cameraSpeedY = _cameraSpeedYSlider.value;
            PlayerPrefs.SetFloat(_CameraSpeedXKey, _cameraSpeedY);
            CameraController.UpdateCameraY(_cameraSpeedY);
            SaveValue();
        }

        public void InvertX()
        {
            _invertCameraX = !_invertCameraX;
            if (_invertCameraX)
            {
                PlayerPrefs.SetInt(_InvertCameraXKey, 1);
                _invertCameraXIndicator.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt(_InvertCameraXKey, 0);
                _invertCameraXIndicator.SetActive(false);   
            }
            CameraController.InvertX(_invertCameraX);
            SaveValue();
        }

        public void InvertY()
        {
            _invertCameraY = !_invertCameraY;
            if (_invertCameraY)
            {
                PlayerPrefs.SetInt(_InvertCameraYKey, 1);
                _invertCameraYIndicator.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt(_InvertCameraYKey, 0);
                _invertCameraYIndicator.SetActive(false);
            }
            CameraController.InvertY(_invertCameraY);
            SaveValue();
        }
        public static void SaveValue()
        {
            PlayerPrefs.Save();
        }
    }
}
