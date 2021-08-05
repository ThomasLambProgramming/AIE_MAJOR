using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public float dotCheck = -0.8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //This does work for a simple cone vision check 
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, directionToPlayer) > dotCheck)
            {
                Debug.Log("PlayerInVision");
            }
            else
            {
                Debug.Log("Not In Vision");
            }
        }
    }
}
