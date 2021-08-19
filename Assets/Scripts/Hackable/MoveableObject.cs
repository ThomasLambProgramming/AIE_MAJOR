using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Interfaces;
using Malicious.Core;
using UnityEngine;

namespace Malicious.Hackable
{
    public class MoveableObject : MonoBehaviour, IHackable
    {
        private Rigidbody objectRigidbody = null;
        [SerializeField] private Transform m_cameraOffset = null;
        private void Start()
        {
            objectRigidbody = GetComponent<Rigidbody>();
        }
        public HackableInformation GiveInformation()
        {
            return new HackableInformation(gameObject, objectRigidbody, m_cameraOffset, ObjectType.MoveableObject);
        }
        public void PlayerExit()
        {
            objectRigidbody.isKinematic = true;
        }
        public void Tick(){}
        public void FixedTick(){}
        

        public void Hacked()
        {
            //Camera offset needs to be set to the players current offset (so it has that no camera rotate thing)
        }
        public void Movement(Vector2 a_input, float a_moveSpeed)
        {
            
        }
        public void SpinMovement(Vector2 a_input, float a_spinSpeed)
        {
            if (a_input != Vector2.zero)
            {
                transform.Rotate(new Vector3(0, a_input.x * a_spinSpeed * Time.deltaTime, 0));
            }
        }

        public void Jump(){}
        public void LeftShiftPressed(){}
        public void SetPlayer(GameObject a_player){}
    }
}
