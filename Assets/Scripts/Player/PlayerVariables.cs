using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Malicious.Interfaces;
using UnityEngine;

namespace Malicious.Player
{
    /// <summary>
    /// This class is to separate variables, so the player class can be divided into multiple smaller classes
    /// for readability without needing to have multiple references to the same information
    /// Its monobehaviour just so its easier to add and see in inspector
    /// </summary>
    [Serializable]
    public class PlayerVariables : MonoBehaviour
    {
        //Singleton player variables to avoid memory overlap
       //public static PlayerVariables m_playerVariables;
        
            
       
        public void Start()
        {
        }
    }
}