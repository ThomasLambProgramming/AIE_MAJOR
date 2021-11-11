using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Malicious.UI
{
    public class VoiceText : MonoBehaviour
    {
        [SerializeField] GameObject _UIObject = null;
        [SerializeField] private Text _text = null;
        public static VoiceText _voiceText = null;

        public bool _displaying = false;
        private void Start()
        {
            _voiceText = this;
        }
        public void DisplayText(int a_logNum)
        {

            switch (a_logNum)
            {
                case 1:
                    _text.text = LogOne();
                    break;
                case 2:
                    _text.text = LogTwo();
                    break;
                case 3:
                    _text.text = LogThree();
                    break;
                case 4:
                    _text.text = LogFour();
                    break;
                case 5:
                    _text.text = LogFive();
                    break;
            }
            Time.timeScale = 0;
            _displaying = true;
            _UIObject.SetActive(true);
        }
        public void HideText()
        {
            Time.timeScale = 1f;
            _UIObject.SetActive(false);
            _displaying = false;
        }

        [ContextMenu("Load First level text")]
        private string LogOne()
        {
            return $"Mad Scientist: \"Recording started. OK, this is test alpha-omega-eleven.  " +
                $"The subject has successfully completed the initial trials, and has broken out of the testing zone. " +
                $"Testing is ongoing, and the subject is unaware that it is still being tested. " + 
                "\nRecording ended.";
        }
        private string LogTwo()
        {
            return $"Mad Scientist: \"Recording started. Test five. The subject is getting smarter, " +
                $"I'm sure of it. it attempted a breakout, which I had to shut down... messily. The Data was corrupted, " +
                $"so I'll have to reload from backup. It's undone weeks of work, darn it!" +
                $"\nRecording ended.";
        }
        private string LogThree()
        {
            return $"Mad Scientist: \"Recording started. This is test seven. The viravore module has been a success. " +
                $"The subject can now absorb parts from other viruses to allow it to take more damage. " +
                $"I haven't informed the subject as to where these 'hearts' come from." +
                $"\nRecording ended.";
        }
        private string LogFour()
        {
            return $"Mad Scientist: \"Recording. Subject Eight has been a little bit snippy, " +
                $"and I've been getting a lot of backchat from them. They have no idea what is coming, " +
                $"so I will have my revenge for that." + 
                $"\nRecording ended.";
        }
        private string LogFive()
        {
            return $"Mad Scientist: \"Recording started. Test alpha-omega - twelve.The subject had completed the trials, " +
                $"but I want to tinker a little bit further, see if I can't make it smarter." +
                $"\nRecording ended.";
        }


    }
}
