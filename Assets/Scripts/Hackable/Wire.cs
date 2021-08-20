using System;
using System.Collections.Generic;
using Malicious.Interfaces;
using UnityEngine;
using Malicious.Core;
using Unity.Mathematics;

namespace Malicious.Hackable
{
    public class Wire : MonoBehaviour, IHackable
    {
        [ContextMenuItem("Add Path Point", "AddPathPoint")] 
        [SerializeField] private bool showPath = false;
        
        #region WireVariables
        private int m_pathIndex = 0;
        public List<Vector3> m_wirePath = new List<Vector3>();
        
        private Quaternion m_rotationGoal = Quaternion.identity;
        private bool m_rotateWireCam = false;
        
        [SerializeField] private int wireCharges = 4;
        [SerializeField] private float wireLength = 5f;
        private bool takingInput = true;
        
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showPath)
            {
                foreach (var point in m_wirePath)
                {
                    Gizmos.DrawSphere(point, 1);
                }
            }
        }
        //Add a path onto the end just makes it easier to make a path not needing to copy paste 
        //positions
        [ContextMenu("AddPathPoint")]
        public void AddPathPoint()
        {
            if (m_wirePath.Count > 0)
            {
                Vector3 newPoint = m_wirePath[m_wirePath.Count - 1];
                m_wirePath.Add(newPoint);
            }
            else
            {
                m_wirePath = new List<Vector3>();
                m_wirePath.Add(gameObject.transform.position);
            }
        }
#endif
        
        public void Hacked()
        {
            //play animation or particle effect
            Vector3 directionToNode = (m_wirePath[1] - m_wirePath[0]).normalized;
            Quaternion newRotation = Quaternion.LookRotation(directionToNode);
            //currentPlayerObject.transform.rotation = newRotation;
            //currentPlayerObject.transform.position = m_wirePath[0];
        }

        public void PlayerExit()
        {
            m_pathIndex = 0;
            m_rotationGoal = Quaternion.identity;
        }
        
        public HackableInformation GiveInformation()
        {
            //as the wire has its own movement object it doesnt need to give much information
            return new HackableInformation(gameObject, null, null, ObjectType.Wire);
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
        }

        public void LeftShiftPressed()
        {
            
        }
        public void Jump()
        {
            if (takingInput)
            {
                takingInput = false;
            }
        }
        public void Movement(Vector2 a_input, float a_moveSpeed)
        {
            if (takingInput)
            {
                if (Vector2.Dot(a_input, Vector2.up) > 0.8f)
                {
                    takingInput = false;
                }
                else if (Vector2.Dot(a_input, Vector2.left) > 0.8f)
                {
                    takingInput = false;
                }
                else if (Vector2.Dot(a_input, Vector2.right) > 0.8f)
                {
                    takingInput = false;
                }
                else if (Vector2.Dot(a_input, Vector2.down) > 0.8f)
                {
                    takingInput = false;
                }

                
                return;
            }
            //if (Vector3.SqrMagnitude(currentPlayerObject.transform.position - m_wirePath[m_pathIndex]) > m_goNextWire)
            //{
            //    Vector3 currentWirePos = currentPlayerObject.transform.position;
            //    currentWirePos = currentWirePos + (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized *
            //        (Time.deltaTime * (m_wireSpeed));
            //    currentPlayerObject.transform.position = currentWirePos;
            //    if (m_rotateWireCam)
            //    {
            //        currentPlayerObject.transform.rotation = Quaternion.RotateTowards(
            //            currentPlayerObject.transform.rotation,
            //            m_rotationGoal,
            //            m_rotateSpeed);
            //        if (currentPlayerObject.transform.rotation == m_rotationGoal)
            //            m_rotateWireCam = false;
            //    }
            //}
            else
            {
                if (m_pathIndex < m_wirePath.Count - 1)
                {
                   // m_pathIndex++;
                   // m_rotationGoal = Quaternion.LookRotation(
                   //     (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized, Vector3.up);
                   // m_rotateWireCam = true;
                }
                else
                {
                    //end of path
                    
                }
            }
        }
        public void OnHackValid(){}
        public void OnHackFalse(){}
        public void SpinMovement(Vector2 a_input, float a_spinSpeed)
        {
            throw new System.NotImplementedException();
        }

        public void SetPlayer(GameObject a_player)
        {
           // currentPlayerObject = a_player;
        }
        
        //Scaling of object (cables)
        //between points needs to be done, need the basic model to do
        //ask daniel or enis.

        private void OnEnable()
        {
            Debug.Log("Enabled");
        }
    }
}
