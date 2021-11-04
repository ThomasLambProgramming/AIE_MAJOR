using Malicious.Core;
using Malicious.Hackable;
using UnityEngine;
using UnityEngine.UI;

namespace Malicious.UI
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _invertCameraXIndicator = null;        
        [SerializeField] private GameObject _invertCameraYIndicator = null;

        [SerializeField] private Slider _cameraSpeedXSlider = null;
        [SerializeField] private Slider _cameraSpeedYSlider = null;
        
        
        private void OnEnable()
        {
            InvertCheck();
        }

        private void InvertCheck()
        {
            if (GlobalData._cameraSettings.InvertX)
            {
                _invertCameraXIndicator.SetActive(true);
                MoveableBlock._invertCamX = true;
                Player._invertCamX = true;
            }
            else
            {
                _invertCameraXIndicator.SetActive(false);
                MoveableBlock._invertCamX = false;
                Player._invertCamX = false;   
            }

            if (GlobalData._cameraSettings.InvertY)
            {
                _invertCameraYIndicator.SetActive(true);
                Player._invertCamY = true;   
            }
            else
            {
                _invertCameraYIndicator.SetActive(false);
                Player._invertCamY = false;   
            }
        }

        //Running on awake so the camera controller can get the values without needing to get references
        void Start()
        {
            _cameraSpeedXSlider.value = GlobalData._cameraSettings.CameraXSpeed;
            _cameraSpeedYSlider.value = GlobalData._cameraSettings.CameraYSpeed;
            
            _cameraSpeedXSlider.onValueChanged.AddListener(delegate {UpdateSliderX();});
            _cameraSpeedYSlider.onValueChanged.AddListener(delegate {UpdateSliderY();});
        }

        private void UpdateSliderX()
        {
            GlobalData._cameraSettings.CameraXSpeed = _cameraSpeedXSlider.value;
            MoveableBlock._spinSpeedCamX = _cameraSpeedXSlider.value;
            Player._spinSpeedCamX = _cameraSpeedXSlider.value;
        }

        private void UpdateSliderY()
        {
            GlobalData._cameraSettings.CameraYSpeed = _cameraSpeedYSlider.value;
            Player._spinSpeedCamY = _cameraSpeedYSlider.value;
        }

        public void InvertX()
        {
            GlobalData._cameraSettings.InvertX = !GlobalData._cameraSettings.InvertX;
                
            if (GlobalData._cameraSettings.InvertX)
            {
                _invertCameraXIndicator.SetActive(true);
                MoveableBlock._invertCamX = true;
                Player._invertCamX = true;
            }
            else
            {
                _invertCameraXIndicator.SetActive(false);
                MoveableBlock._invertCamX = false;
                Player._invertCamX = false;
            }
        }

        public void InvertY()
        {
            GlobalData._cameraSettings.InvertY = !GlobalData._cameraSettings.InvertY;
                
            if (GlobalData._cameraSettings.InvertY)
            {
                _invertCameraYIndicator.SetActive(true);
                Player._invertCamY = true;
            }
            else
            {
                _invertCameraYIndicator.SetActive(false);
                Player._invertCamY = false;
            }
        }
    }
}
