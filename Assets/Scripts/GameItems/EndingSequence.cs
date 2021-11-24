using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malicious.Core;
using Malicious.UI;

namespace Malicious.GameItems
{
    public class EndingSequence : MonoBehaviour
    {
        [SerializeField] Transform _playerCameraOffset = null;
        [SerializeField] Transform _cameraTransform = null;
        [SerializeField] FadeTransition _fadeObject = null;
        [SerializeField, TextArea] List<string> _endingText = new List<string>();
        [SerializeField] List<float> _waitTimes = new List<float>();
        [SerializeField] Subtitles _subtitlesObject = null;

        // Start is called before the first frame update
        void Start()
        {
            CameraController._currentHackableCamera.LookAt = _cameraTransform;
            CameraController._currentHackableCamera.Follow = _cameraTransform;
        }

        public void EndingPlay()
        {
            _cameraTransform.position = _playerCameraOffset.position;
            _cameraTransform.rotation = _playerCameraOffset.rotation;

            CameraController._currentHackableCamera.LookAt = _cameraTransform;
            CameraController._currentHackableCamera.Follow = _cameraTransform;

            _subtitlesObject.GiveText(_endingText, _waitTimes);


            
        }

        private IEnumerator WaitToFadeOut(float a_float)
        {
            yield return new WaitForSeconds(a_float);
            _fadeObject.FadeOut();
        }
    }
}
