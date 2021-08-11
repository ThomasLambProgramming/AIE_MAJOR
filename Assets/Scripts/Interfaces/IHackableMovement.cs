using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.Interfaces
{
    public enum MovementHackables
    {
        MoveableObject = 1,
        GroundAgent = 2, 
        FlyingAgent = 3,
        Wire = 4
    }
    public class MovementObjectInformation
    {
        public GameObject hackableObject = null;
        public Rigidbody hackableRigigbody = null;

        public MovementObjectInformation(GameObject a_gameObject, Rigidbody a_rigidbody = null)
        {
            hackableObject = a_gameObject;
            hackableRigigbody = a_rigidbody;
        }
    }
    /// <summary>
    /// Movement hackables are all the types of objects that
    /// change the players movement, eg drone and moveable object
    /// </summary>
    public interface IHackableMovement
    {
        //this is to avoid get component in the player script
        public MovementObjectInformation GiveObjectInformation();
        //Function for when the player hacks the movement object
        
        
        
        //dont forget to disable iskinematic for objects 
        public void Hacked();
        //for when the player exits the hacked object (so ai can return to path and etc)
        public void PlayerLeft();

        //This is so the needed information can be taken from get component eg get path from wire
        public MovementHackables TypeOfHackable();
    }
}
