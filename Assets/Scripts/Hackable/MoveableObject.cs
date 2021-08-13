using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Interfaces;
using UnityEngine;

namespace Malicious.Hackable
{
    public class MoveableObject : MonoBehaviour, IHackableMovement
    {
        private Rigidbody objectRigidbody = null;

        private void Start()
        {
            objectRigidbody = GetComponent<Rigidbody>();
        }

        public MovementObjectInformation GiveObjectInformation()
        {
            return new MovementObjectInformation(gameObject, objectRigidbody);
        }
        //Function for when the player hacks the movement object



        //dont forget to disable iskinematic for objects 
        public void Hacked()
        {
            
        }

        //for when the player exits the hacked object (so ai can return to path and etc)
        public void PlayerLeft()
        {
            //objectRigidbody.isKinematic = true;
        }

    //This is so the needed information can be taken from get component eg get path from wire
        public MovementHackables TypeOfHackable() => MovementHackables.MoveableObject;

        private void OnTriggerEnter(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                //Malicious.Player.PlayerController.MainPlayer.SetMoveable(this);
            }
        }
        private void OnTriggerExit(Collider a_other)
        {
            if (a_other.transform.CompareTag("Player"))
            {
                //Malicious.Player.PlayerController.MainPlayer.SetMoveable(null);
            }
        }
    }
}
