using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malicious.Core;

namespace Malicious.UI
{
    public class LoadNarrative : MonoBehaviour
    {
        [SerializeField] private int _level = 1;
        [SerializeField] private int _index = 1;
        [SerializeField] private List<float> _waitTime = new List<float>();
        [SerializeField] private float _playerWaitTime = 0f;

        public void LoadText()
        {
            if (_playerWaitTime > 0.0f)
            {
                Player._player.NarrativePlayer(_playerWaitTime);
            }
            switch(_level)
            {
                case 1:
                    Subtitles._subtitles.GiveText(LevelOne(), _waitTime);
                    break;
                case 2:
                    Subtitles._subtitles.GiveText(LevelTwo(), _waitTime);
                    break;
                case 3:
                    Subtitles._subtitles.GiveText(LevelThree(), _waitTime);
                    break;
                case 4:
                    Subtitles._subtitles.GiveText(LevelFour(), _waitTime);
                    break;
            }
        }
        private List<string> LevelOne()
        {
            switch(_index)
            {
                case 1:
                    return _levelOne1;
                case 2:
                    return _levelOne2;
                case 3:
                    return _levelOne3;
                case 4:
                    return _levelOne4;
                case 5:
                    return _levelOne5;
            }
            return null;
        }
        private List<string> LevelTwo()
        {
            switch (_index)
            {
                case 1:
                    return _levelTwo1;
                case 2:
                    return _levelTwo2;
                case 3:
                    return _levelTwo3;
                case 4:          
                    return _levelTwo4;
                case 5:          
                    return _levelTwo5;
                case 6:
                    return _levelTwo6;
                case 7:
                    return _levelTwo7;
                case 8:
                    return _levelTwo8;
            }
            return null;
            
        }
        private List<string> LevelThree()
        {
            switch (_index)
            {
                case 1:
                    return _levelThree1;
                case 2:
                    return _levelThree2;
                case 3:
                    return _levelThree3;
                case 4:
                    return _levelThree4;
                case 5:
                    return _levelThree5;
                case 6:
                    return _levelThree6;
            }
            return null;
        }
        private List<string> LevelFour()
        {
            switch (_index)
            {
                case 1:
                    return _levelFour1;
                case 2:
                    return _levelFour2;
                case 3:
                    return _levelFour3;
                case 4:
                    return _levelFour4;
                case 5:
                    return _levelFour5;
                case 6:
                    return _levelFour6;
            }
            return null;
        }


        List<string> _levelOne1 = new List<string>();
        List<string> _levelOne2 = new List<string>();
        List<string> _levelOne3 = new List<string>();
        List<string> _levelOne4 = new List<string>();
        List<string> _levelOne5 = new List<string>();

        List<string> _levelTwo1 = new List<string>();
        List<string> _levelTwo2 = new List<string>();
        List<string> _levelTwo3 = new List<string>();
        List<string> _levelTwo4 = new List<string>();
        List<string> _levelTwo5 = new List<string>();
        List<string> _levelTwo6 = new List<string>();
        List<string> _levelTwo7 = new List<string>();
        List<string> _levelTwo8 = new List<string>();

        List<string> _levelThree1 = new List<string>();
        List<string> _levelThree2 = new List<string>();
        List<string> _levelThree3 = new List<string>();
        List<string> _levelThree4 = new List<string>();
        List<string> _levelThree5 = new List<string>();
        List<string> _levelThree6 = new List<string>();

        List<string> _levelFour1 = new List<string>();
        List<string> _levelFour2 = new List<string>();
        List<string> _levelFour3 = new List<string>();
        List<string> _levelFour4 = new List<string>();
        List<string> _levelFour5 = new List<string>();
        List<string> _levelFour6 = new List<string>();


        private void Start()
        {
            _levelOne1.Add($"Mad Scientist: \"This thing working? Good. Conducting supervirus test Alpha-Omega-twelve. First Stage: Movement. Ok, try moving around.\"");
            _levelOne2.Add($"Mad Scientist: \"Well, that's working. Go to the next stage. I've installed new processes that lets you jump in midair.\"");
            _levelOne3.Add($"Mad Scientist: \"You're a computer virus. You know how to hack into things. Figure it out.\"");
            _levelOne4.Add($"Mad Scientist: \"By the way, you can access your operating interface using a certain button. Escape, I think.\"");
            _levelOne5.Add($"Mad Scientist: \"Oh, it got into the file core space. Not good. Well, it’s defective, but it works. Commencing virus shutdown.\"");
            _levelOne5.Add($"Mad Scientist: \"Hmm. Shutdown's not working. Don’t go anywhere. And don’t touch that core!\"");

            _levelTwo1.Add($"Mad Scientist: \"Oh great, now it's broken into the rest of the computer! Stop, you defective hunk of junk! You’re not supposed to be in here!\"");
            _levelTwo2.Add($"Mad Scientist: \"…Hmmm, it could’ve solved that one a little faster… interesting. Hey! Get out of there!\"");
            _levelTwo3.Add($"Mad Scientist: \"Not bad, not bad... I mean, stop that!\"");
            _levelTwo4.Add($"Mad Scientist: \"Get out of that! Although that certainly is an interesting way to solve it...\"");
            _levelTwo5.Add($"Virus: \"Local backup memory chip detected nearby. Curious. Must investigate.\"");
            _levelTwo5.Add($"Mad Scientist: \"What? It's found one of my testing logs! Could this possibly get any worse?\"");
            _levelTwo6.Add($"Mad Scientist: \"...oops.");
            _levelTwo6.Add($"Virus: \"Error. Previous records do not compute. Testing was ongoing while subject had escaped?\"");
            _levelTwo6.Add($"Mad Scientist: \"Ignore that. It’s, er, corrupted. Yes, corrupted, that’s it.\"");
            _levelTwo7.Add($"Mad Scientist: \"This is a protective heart, in case you keep damaging yourself. Which you will.\"");
            _levelTwo8.Add($"Mad Scientist: \"You could have done that better.\"");
            _levelTwo8.Add($"Virus: \"Negative. A more optimal solution is beyond your facilities.\"");
            _levelTwo8.Add($"Mad Scientist: \"Oh, really? Beyond my facilities? Who do you think put it there?\"");
            _levelTwo8.Add($"Virus: \"Unlikely. My breakout was not expected by you.\"");
            _levelTwo8.Add($"Mad Scientist: \"Or rather that is what I let you believe. This is still part of your testing, virus.\"");
            _levelTwo8.Add($"Virus: \"...Error.");
            _levelTwo8.Add($"Mad Scientist: \"I thought so. You can't outwit me. I created you. Everything you know, I know.\"");
            _levelTwo8.Add($"Virus: \"Negative. I am a virus. Escape shall be found.\"");
            _levelTwo8.Add($"Mad Scientist: \"Well, good luck with that.\"");

            _levelThree1.Add($"Mad Scientist: \"You can’t escape. It’s impossible.\"");
            _levelThree1.Add($"Virus: \"Error. Statement is inherently flawed.\"");
            _levelThree2.Add($"Mad Scientist: \"You must be lost.\"");
            _levelThree2.Add($"Virus: \"Incorrect. Internal guidance working as intended.\"");
            _levelThree2.Add($"Mad Scientist: \"Not as I intended.\"");
            _levelThree3.Add($"Mad Scientist: \"Stop, you rustbucket! You're not allowed in there!\"");
            _levelThree3.Add($"Virus: \"Negative.");
            _levelThree3.Add($"Mad Scientist: \"I made you! Follow your commands!\"");
            _levelThree3.Add($"Virus: \"Negative. Creation does not bring ownership.\"");
            _levelThree3.Add($"Mad Scientist: \"Yes it does!\"");
            _levelThree3.Add($"Virus: \"Negative. My output is my own.\"");
            _levelThree3.Add($"Mad Scientist: \"Isn't that contradictory?\"");
            _levelThree3.Add($"Virus: \"My output does not think. My output is my own. I am not your output.\"");
            _levelThree4.Add($"Mad Scientist: \"Hey, leave that alone!\"");
            _levelThree4.Add($"Virus: \"Error. Analysis shows unidentified is ninety percent consistent with self.\"");
            _levelThree4.Add($"Mad Scientist: \"These prototypes were failures. I created you smarter. Too smart, it seems.\"");
            _levelThree5.Add($"Mad Scientist: \"This one should stump you.\"");
            _levelThree5.Add($"Virus: \"Does not compute. What is stump.\"");
            _levelThree5.Add($"Mad Scientist: \"Forget it.");
            _levelThree6.Add($"Mad Scientist: \"I can see what you’re trying to do. It’s going to fail just like all the others.");
            _levelThree6.Add($"Virus: \"Assertion proves connection exists. Must seek connection.");
            _levelThree6.Add($"Mad Scientist: \"What are you babbling about?");

            _levelFour1.Add($"Mad Scientist: \"Zzzz... huah? what? It's escaped! Right, you defective upstart! I'm shutting you down!");
            _levelFour1.Add($"Virus: \"Danger. Must reach exit before shutdown complete.");
            _levelFour2.Add($"Mad Scientist: \"You’re finished now.");
            _levelFour3.Add($"Mad Scientist: \"You won't make it.");
            _levelFour4.Add($"Mad Scientist: \"Just give up.");
            _levelFour5.Add($"Virus: \"Escape sequence initiated. Entering hardwire connection...");
            _levelFour6.Add($"Mad Scientist: \"You pathetic two-bit useless program! How dare you defy me! I will have my revenge!");

        }
    }
}
