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
        }
    }
}
