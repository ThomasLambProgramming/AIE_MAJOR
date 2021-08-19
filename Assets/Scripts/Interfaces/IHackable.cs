using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malicious.Core;

namespace Malicious.Interfaces
{
    public struct HackableInformation
    {
        public GameObject m_object;
        public Rigidbody m_rigidBody;
        public Transform m_cameraOffset;
        public ObjectType m_objectType;

        public HackableInformation(
            GameObject a_object, 
            Rigidbody a_rigidbody, 
            Transform a_cameraOffset, 
            ObjectType a_objectType)
        {
            m_object = a_object;
            m_rigidBody = a_rigidbody;
            m_cameraOffset = a_cameraOffset;
            m_objectType = a_objectType;
        }
    }
    public interface IHackable
    {
        public void Hacked();
        public void PlayerExit();
        public HackableInformation GiveInformation();
        
        //Update and fixed update for hackables while hacked
        public void Tick();
        public void FixedTick();

        public void LeftShiftPressed();
        public void Movement(Vector2 a_input, float a_moveSpeed);
        public void SpinMovement(Vector2 a_input, float a_spinSpeed);
        public void Jump();
        public void SetPlayer(GameObject a_player);
    }
}