using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public List<Vector3> path = new List<Vector3>();
    
    private void OnDrawGizmos()
    {
        if (Selection.activeObject == gameObject)
        {
            foreach (var point in path)
            {
                Gizmos.DrawSphere(point, 1);
            }
        }
    }
    
}
