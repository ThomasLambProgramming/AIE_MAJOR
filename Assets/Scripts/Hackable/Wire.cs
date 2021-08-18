using System.Collections.Generic;
using Malicious.Interfaces;
using UnityEngine;
using Malicious.Core;

namespace Malicious.Hackable
{
    public class Wire : MonoBehaviour, IHackable
    {
        [ContextMenuItem("Add Path Point", "AddPathPoint")] [SerializeField]
        private List<Vector3> path = null;
        [SerializeField] private bool showPath = false;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showPath)
            {
                foreach (var point in path)
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
            if (path.Count > 0)
            {
                Vector3 newPoint = path[path.Count - 1];
                path.Add(newPoint);
            }
            else
            {
                path = new List<Vector3>();
                path.Add(gameObject.transform.position);
            }
        }
#endif
        
        public void Hacked()
        {
            //play animation or particle effect
        }
        public void PlayerExit(){}
        
        public HackableInformation GiveInformation()
        {
            //as the wire has its own movement object it doesnt need to give much information
            return new HackableInformation(gameObject, null, null, ObjectType.Wire);
        }
        public List<Vector3> GivePath()
        {
            return path;
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
        //Scaling of object (cables)
        //between points needs to be done, need the basic model to do
        //ask daniel or enis.
        
        
    }
}
