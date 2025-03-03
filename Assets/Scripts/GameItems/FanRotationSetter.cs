﻿using UnityEngine;
using UnityEngine.Events;

namespace Malicious.GameItems
{
    public class FanRotationSetter : MonoBehaviour
    {
        [SerializeField] private Fan _fanToAffect = null;
        [SerializeField] private Vector3 _goalRotation = Vector3.zero;
        
        [SerializeField] private Vector3 _goalRotationExit = Vector3.zero;


        public void RotateFan()
        {
            _fanToAffect.RotateFan(_goalRotation);
        }
        public void RotateFan2()
        {
            _fanToAffect.RotateFan(_goalRotationExit);
        }
        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawLine(_fanToAffect.transform.position, _fanToAffect.transform.position + _goalRotation * 4);
        //}
    }
}