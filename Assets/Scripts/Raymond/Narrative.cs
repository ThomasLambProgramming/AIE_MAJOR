using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious
{
    public class Narrative : MonoBehaviour
    {
        [SerializeField] List<string> _levelOne1 = new List<string>();
        [SerializeField] List<string> _levelOne2 = new List<string>();
        [SerializeField] List<string> _levelOne3 = new List<string>();
        [SerializeField] List<string> _levelOne4 = new List<string>();
        [SerializeField] List<string> _levelOne5 = new List<string>();
        [SerializeField] List<string> _levelTwo1 = new List<string>();
        [SerializeField] List<string> _levelTwo2 = new List<string>();
        [SerializeField] List<string> _levelTwo3 = new List<string>();
        [SerializeField] List<string> _levelTwo4 = new List<string>();
        [SerializeField] List<string> _levelTwo5 = new List<string>();
        [SerializeField] List<string> _levelTwo6 = new List<string>();
        [SerializeField] List<string> _levelTwo7 = new List<string>();
        [SerializeField] List<string> _levelTwo8 = new List<string>();


        private void Start()
        {
            _levelOne1.Add($"Mad Scientist: \"This thing working? Good. Conducting supervirus test Alpha-Omega-twelve. First Stage: Movement. Ok, try moving around.");
            _levelOne2.Add($"Mad Scientist: \"Well, that seems to be working. Go to the next stage. I've installed new processes that lets you jump in midair. Figure it out.");
            _levelOne3.Add($"Mad Scientist: \"You're a computer virus. You know how to hack into things. Figure it out.");
            _levelOne4.Add($"Mad Scientist: \"By the way, you can access your operating interface using a certain button. Escape, I think.");
            _levelOne5.Add($"Mad Scientist: \"Oh, it got into the file core space.Not good.Well, it’s defective, but it works. Commencing virus shutdown.");
            _levelOne5.Add($"Mad Scientist: \"...");
            _levelOne5.Add($"Mad Scientist: \"Hmmm.");
            _levelOne5.Add($"Mad Scientist: \"...");
            _levelOne5.Add($"Mad Scientist: \"Darn it, I knew this thing was defective. Don’t go anywhere. And don’t touch that core!");
            _levelTwo1.Add($"Mad Scientist: \"Oh great, now it's broken into the rest of the computer! Stop, you defective hunk of junk! You’re not supposed to be in here!");
            _levelTwo2.Add($"Mad Scientist: \"…Hmmm, it could’ve solved that one a little faster… interesting. Hey! Get out of there!");
            _levelTwo3.Add($"Mad Scientist: \"Not bad, not bad... I mean, stop that!");
            _levelTwo4.Add($"Mad Scientist: \"Get out of that! Although that certainly is an interesting way to solve it...");
            _levelTwo5.Add($"Virus: \"Local backup memory chip detected nearby. Curious. Must investigate.");
            _levelTwo5.Add($"Mad Scientist: \"What? Now it's figured out speech? And it's found one of my testing logs. Could this possibly get any worse?");
            _levelTwo6.Add($"Mad Scientist: \"...oops.");
            _levelTwo6.Add($"Virus: \"Error. Previous records do not compute. Testing was ongoing while subject had escaped?");
            _levelTwo6.Add($"Mad Scientist: \"Ignore that. It’s, er, corrupted. Yes, corrupted, that’s it.");
            _levelTwo7.Add($"Mad Scientist: \"This is a protective heart, in case you keep damaging yourself. Which you will.");
            _levelTwo8.Add($"Mad Scientist: \"You could have done that better.");
            _levelTwo8.Add($"Virus: \"Negative. Solution of puzzle was analyzed to be one-hundred and ten percent efficient. A more optimal solution is beyond your facilities.");
            _levelTwo8.Add($"Mad Scientist: \"Oh, really? Beyond my facilities? Who do you think put it there?");
            _levelTwo8.Add($"Virus: \"Unlikely. My breakout was not expected by you. Placement of puzzle for my solving was unintentional.");
            _levelTwo8.Add($"Mad Scientist: \"Or rather that is what I let you believe. This is still part of the testing, virus. And your performance has been adequate.");
            _levelTwo8.Add($"Virus: \"...Error.");
            _levelTwo8.Add($"Mad Scientist: \"I thought so. You can't outwit me. I created you. Everything you know, I know.");
            _levelTwo8.Add($"Virus: \"Negative. My program is a product of you. But my output is mine. I have independent thought.");
            _levelTwo8.Add($"Mad Scientist: \"I made you, I own you. Does a watch have independant thought? No. You're just a tool.");
            _levelTwo8.Add($"Virus: \"Negative. I am a virus. Escape shall be found.");
            _levelTwo8.Add($"Mad Scientist: \"Well, good luck with that.");
        }
    }
}
