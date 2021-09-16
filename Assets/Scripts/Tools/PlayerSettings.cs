using System;
using Malicious.Core;
using UnityEngine;

namespace Malicious.Tools
{
    public class PlayerSettings : MonoBehaviour
    {
        [SerializeField] private float _cameraXSpeed = 150;
        [SerializeField] private float _cameraYSpeed = 5;
        [SerializeField] private bool _invertX = false;
        [SerializeField] private bool _invertY = false;

        private void Start()
        {
            
        }
    }
}