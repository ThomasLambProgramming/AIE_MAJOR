using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.Interfaces
{
    public struct HackableInformation
    {
        public GameObject m_object;
        public Rigidbody m_rigidBody;
        public Transform m_cameraOffset;

        public HackableInformation(GameObject a_object, Rigidbody a_rigidbody, Transform a_transform)
        {
            m_object = a_object;
            m_rigidBody = a_rigidbody;
            m_cameraOffset = a_transform;
        }
    }
    public interface IHackable
    {
        public void Hacked();
        public void PlayerExit();
        public HackableInformation GiveInformation();
    }
}


/*
 * Movement scheme changing from player to moveable or wire
 * function for each
 * or just running unity event
 * that way i can just have if interactable != null interactable.hacked();
 *
 *
 *
 * ----PLAYER-----
 * Enter wire, Exit Wire, Wire Jump Override
 *
 * Movement function for ground, flying and block with options for designers
 */
