using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Malicious.Hackable
{
    public class Wire : MonoBehaviour
    {
        public List<Vector3> path = new List<Vector3>();
    
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.activeObject == gameObject)
            {
            }
            foreach (var point in path)
            {
                Gizmos.DrawSphere(point, 1);
            }
        }
#endif
        public List<Vector3> GivePath()
        {
            return path;
        }
    }
}
