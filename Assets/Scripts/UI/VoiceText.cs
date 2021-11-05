using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Malicious
{
    public class VoiceText : MonoBehaviour
    {
        // Start is called before the first frame update
        
        void Start()
        {
            Debug.Log(_LevelOne[0]);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public static string[] _LevelOne =
        {
            $"Mad Scientist: \"This thing working? Good. Conducting supervirus test Alpha-Omega-twelve. First Stage: Movement. Ok, try moving around.",
            $"Mad Scientist: \"Well, that seems to be working. Right. Go to the next stage. You'll know what to do.",

        };
        public static string[] _LevelTwo =
        {

        };
        public static string[] _LevelThree =
        {

        };
        public static string[] _LevelFour =
        {

        };




    }
}
