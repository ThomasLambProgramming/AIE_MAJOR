using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Malicious.Hackable
{
    public class Wire : MonoBehaviour
    {
        [ContextMenuItem("Add Path Point", "AddPathPoint")]
        [SerializeField] private List<Vector3> path = new List<Vector3>();
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
        public List<Vector3> GivePath()
        {
            return path;
        }
        
        
        //Scaling of object (cables)
        //between points needs to be done, need the basic model to do
        //ask daniel or enis.
        
        
    }
}
