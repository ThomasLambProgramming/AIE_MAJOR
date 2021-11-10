using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious
{
    public class Narrative : MonoBehaviour
    {
        [SerializeField] List<string> _levelOne1 = new List<string>();
       


        private void Start()
        {
            _levelOne1.Add($"Mad Scientist: \"This thing working? Good. Conducting supervirus test Alpha-Omega-twelve. First Stage: Movement. Ok, try moving around.");
        }
    }
}
