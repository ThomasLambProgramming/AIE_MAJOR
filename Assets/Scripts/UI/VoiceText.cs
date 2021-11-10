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

        [SerializeField] List<string> _levelOneText = new List<string>();


        public bool _displaying = false;
        private void Start()
        {
            _voiceText = this;
        }

        [ContextMenu("TextTested")]
        public void Testing()
        {

        }

        public void DisplayText(int a_level, int a_index)
        {

            switch (a_level)
            {
                case 0:
                    //_text.text = _LevelOne[a_index];
                    break;
                case 1:
                    _text.text = _LevelTwo[a_index];
                    break;
                case 2:
                    _text.text = _LevelThree[a_index];
                    break;
                case 3:
                    _text.text = _LevelFour[a_index];
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
        public void _LevelOne()
        {
            List<string> test = new List<string>();
            test.Add($"Mad Scientist: \"This thing working? Good. Conducting supervirus test Alpha-Omega-twelve. First Stage: Movement. Ok, try moving around.");
            test.Add($"Mad Scientist: \"Well, that seems to be working. Go to the next stage. I've installed new processes that lets you jump in midair. Figure it out.");
            test.Add($"Mad Scientist: \"You're a computer virus. You know how to hack into things. Figure it out.");
            test.Add($"Mad Scientist: \"By the way, you can access your operating interface using a certain button. Escape, I think.");
            test.Add($"Mad Scientist: \"H-hey! I said don’t touch the core! Stop! Get out of there! That’s not for you!");
            test.Add($"Mad Scientist: \"Oh, it got into the file core space. Not good. Well, it’s defective, but it works. Commencing virus shutdown.");
            test.Add("Mad Scientist: \"... ");
            test.Add("Mad Scientist: \"Hmmm.");
            test.Add("Mad Scientist: \"...");
            test.Add("Mad Scientist: \"Darn it, I knew this thing was defective. Don’t go anywhere. And don’t touch that core!\"");

            Subtitles._subtitles.GiveText(test);
            
        }
        public static string[] _LevelTwo =
        {
            $"Mad Scientist: \"You can’t escape. It’s impossible." + "\n" + 
            "Virus: \"Error. Initial analysis incomplete. Statement is inherently flawed.",

            $"Mad Scientist: \"You must be lost." + "\n" +
            "Virus: \"Incorrect. Internal guidance working as intended." + "\n" +
            "Mad Scientist: \"Not as I intended.",


        };
        public static string[] _LevelThree =
        {

        };
        public static string[] _LevelFour =
        {

        };

        public static string[] _MemoryLogs =
        {
            $"Mad Scientist: \"Recording started. OK, this is test alpha-omega-eleven.  The subject has successfully completed the initial trials, " +
                $"and has broken out of the testing zone. Testing is ongoing, and the subject is unaware that it is still being tested." +
                "\n Recording ended.",

            $"Mad Scientist: \"Recording started. Test five. The subject is getting smarter, I'm sure of it. it attempted a breakout, " +
                $" which I had to shut down... messily. The Data was corrupted, so I'll have to reload from backup. It's undone weeks of work, darn it!" +
            "\n Recording ended.",

            $"Mad Scientist: \"Recording started. This is test seven. Another breakout attempt. I can't keep deleting them every time they try to get out- this is the third time now! I'm going to have to figure something out." +
            "\n Recording ended.",

            $"Mad Scientist: \"Recording. Subject Eight has been a little bit snippy, and I've been getting a lot of backchat from them. They have no idea what is coming, so I will have my revenge for that." +
            "\n Recording ended.",

            $"Mad Scientist: \"Recording started. Test alpha-omega-twelve. The subject had completed the trials, but I want to tinker a little bit further, see if I can't make it smarter." +
            "Recording ended."
        };


    }
}
