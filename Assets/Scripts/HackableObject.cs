using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HackedType
{
    ENEMY = 1,
    MOVEABLEOBJECT = 2,
    CONTROLPANEL = 3
}
public class HackableObject : MonoBehaviour
{
    public float dotAllowance = 0.9f;
    public float indicatorDistance = 1f;
    public float indicatorSpinSpeed = 10f;
    
    public GameObject indiciator = null;
    private PlayerMovement playerScript = null;
    private BoxCollider[] colliders = null;

    [SerializeField] private GameObject cameraFollow = null;
    
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private bool isMoveable = false;
    [SerializeField] private bool isControlPanel = false;
    private void OnTriggerStay(Collider a_other)
    {
        //This is universal for all hackable objects to have the indicator
        //later expand this to have rotation and position allowances
        if (a_other.transform.CompareTag("Player"))
        {
            Vector3 playerToObject = (transform.position - a_other.transform.position).normalized;
            Vector3 objectToPlayer = (a_other.transform.position - transform.position).normalized;
            if (Vector3.Dot(playerToObject, a_other.transform.forward) > dotAllowance)
            {
                indiciator.SetActive(true);
                Vector3 indicatorPosition = transform.position + objectToPlayer * indicatorDistance;
                indiciator.transform.position = indicatorPosition;
                indiciator.transform.Rotate(new Vector3(0, indicatorSpinSpeed * Time.deltaTime, 0));
                playerScript.SetInteractable(this);
            }
            else
            {
                playerScript.RemoveInteractable();
                indiciator.SetActive(false);
            }
        }
    }

    private void Start()
    {
        //there is one for physical stopping and one for the interacting range
        colliders = GetComponents<BoxCollider>();
    }


    public void BeingHacked(out HackedType a_hackedType, out Transform a_cameraFollow)
    {
        if (isEnemy)
        {
            a_hackedType = HackedType.ENEMY;
            a_cameraFollow = cameraFollow.transform;
        }
        else if (isMoveable)
        {
            //moveable object
            a_hackedType = HackedType.MOVEABLEOBJECT;
            a_cameraFollow = cameraFollow.transform;

        }
        else
        {
            a_cameraFollow = null;
            a_hackedType = HackedType.CONTROLPANEL;
            //control panel
            //move it up as an indicator of working
            transform.Translate(0, 2, 0);

            playerScript.RemoveInteractable();
            indiciator.SetActive(false);
            //stops the player from interacting with it again
            foreach (var boxCollider in colliders)
            {
                if (boxCollider.isTrigger)
                    boxCollider.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider a_other)
    {
        if (a_other.transform.CompareTag("Player"))
        {
            playerScript = a_other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit(Collider a_other)
    {
        if (a_other.transform.CompareTag("Player"))
        {
            playerScript = null;
            if (indiciator.activeInHierarchy)
                indiciator.SetActive(false);
        }
    }

}
