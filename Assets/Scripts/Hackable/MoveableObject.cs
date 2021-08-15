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
       
        public void Hacked()
        {
            
        }

        public void PlayerExit()
        {
            objectRigidbody.isKinematic = true;
        }

        private void OnTriggerEnter(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                Malicious.Player.PlayerController.PlayerControl.SetInteractable(this);
            }
        }
        private void OnTriggerExit(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                Malicious.Player.PlayerController.PlayerControl.SetInteractable(null);
            }
        }
    }
}
