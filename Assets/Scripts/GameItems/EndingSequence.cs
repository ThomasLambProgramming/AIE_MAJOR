using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malicious.Core;
using Malicious.UI;
using Malicious.Tools;

namespace Malicious.GameItems
{
    public class EndingSequence : MonoBehaviour
    {
        [SerializeField] FadeTransition _fadeObject = null;
        [SerializeField, TextArea] List<string> _endingText = new List<string>();
        [SerializeField] List<float> _waitTimes = new List<float>();
        [SerializeField] Subtitles _subtitlesObject = null;
        [SerializeField] SceneManage _sceneManage = null;
        [SerializeField] float _totalWaitTime = 6f;
        [SerializeField] GameObject _playerObject = null;
        [SerializeField] float _finWaitTime = 5f;
        [SerializeField] GameObject _finText = null;

        [SerializeField] Transform _playerMoveGoal = null;
        

        public void EndingPlay()
        {
            CameraController.ChangeCamera(ObjectType.PointOfInterest, null);
            _subtitlesObject.GiveText(_endingText, _waitTimes);

            _playerObject.GetComponent<Player>().EndingSequence(_playerMoveGoal.position);

            StartCoroutine(WaitToFadeOut(_totalWaitTime));
        }

        private IEnumerator WaitToFadeOut(float a_float)
        {
            yield return new WaitForSeconds(a_float);
            _fadeObject.FadeOut();
            yield return new WaitForSeconds(2);
            _finText.SetActive(true);
            yield return new WaitForSeconds(_finWaitTime);
            _sceneManage.LoadMenu();
        }
    }
}
