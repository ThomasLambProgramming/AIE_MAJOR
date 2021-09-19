using System;
using Malicious.Core;
using UnityEngine;

namespace Malicious.GameItems
{
    public class Spike : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //To make it simple mainly because losing all your hp just takes you back 
                //to the checkpoint hitting the spike is the same as dying so there was no need
                //for anything special
                GameEventManager.SpikeHit();
            }
        }
    }
}