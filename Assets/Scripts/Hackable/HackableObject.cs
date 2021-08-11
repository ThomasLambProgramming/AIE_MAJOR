using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    public enum HackedType
    {
        Enemy = 1,
        MoveableObject = 2,
        Wire = 3,
        INVALID = 4
    }
    
    [SelectionBase]
    public class HackableObject : MonoBehaviour
    {
        private PlayerMovement playerScript = null;
        private BoxCollider[] colliders = null;

        private void Start()
        {
            //there is one for physical stopping and one for the interacting range
            colliders = GetComponents<BoxCollider>();
        }

        private HackedType objectType = HackedType.Enemy;

        public void BeingHacked(out HackedType a_hackedType)
        {
            if (objectType == HackedType.Enemy)
            {
                a_hackedType = HackedType.Enemy;
            }
            else if (objectType == HackedType.MoveableObject)
            {
                //moveable object
                a_hackedType = HackedType.MoveableObject;
            }
            else if (objectType == HackedType.Wire)
            {
                a_hackedType = HackedType.Wire;
            }
            else
            {
                a_hackedType = HackedType.INVALID;
            }

        }

        private void OnTriggerEnter(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                playerScript = a_other.transform.parent.GetComponent<PlayerMovement>();
                playerScript.UpdateInteractable(this);
            }
        }
    }
}