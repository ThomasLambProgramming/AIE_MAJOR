using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    public enum HackedType
    {
        Enemy = 1,
        MoveableObject = 2,
        ControlPanel = 3,
        Lever = 4,
        Wire = 5,
        INVALID = 6
    
    }
    [SelectionBase]
    public class HackableObject : MonoBehaviour
    {
        public float dotAllowance = 0.9f;
        public float indicatorDistance = 1f;
        public float indicatorSpinSpeed = 10f;
    
        public GameObject indiciator = null;
        private PlayerMovement playerScript = null;
        private BoxCollider[] colliders = null;

        [SerializeField] private GameObject cameraFollow = null;
    
        [SerializeField] private HackedType objectType = HackedType.Enemy;

        [Header("These events will only run when it is of control panel type")]
        [SerializeField] private UnityEvent onHackedEvent;

        private bool leverUsed = false;
        private void OnTriggerStay(Collider a_other)
        {
            //This is universal for all hackable objects to have the indicator
            //later expand this to have rotation and position allowances
            if (a_other.transform.CompareTag("Player"))
            {
                Vector3 playerToObject = (transform.position - a_other.transform.position).normalized;
                Vector3 objectToPlayer = (a_other.transform.position - transform.position).normalized;
                if (indiciator != null)
                {
                    if (leverUsed)
                    {
                        indiciator.SetActive(false);
                    }
                    else if (Vector3.Dot(playerToObject, a_other.transform.forward) > dotAllowance)
                    {
                        indiciator.SetActive(true);
                        Vector3 indicatorPosition = transform.position + objectToPlayer * indicatorDistance;
                        indicatorPosition.y = (transform.position.y + a_other.transform.position.y) / 2 + 2;

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
        }

        private void Start()
        {
            //there is one for physical stopping and one for the interacting range
            colliders = GetComponents<BoxCollider>();
        }


        public void BeingHacked(out HackedType a_hackedType, out Transform a_cameraFollow)
        {
            if (objectType == HackedType.Enemy)
            {
                a_hackedType = HackedType.Enemy;
                a_cameraFollow = cameraFollow.transform;
            }
            else if (objectType == HackedType.MoveableObject)
            {
                //moveable object
                a_hackedType = HackedType.MoveableObject;
                a_cameraFollow = cameraFollow.transform;

            }
            else if (objectType == HackedType.ControlPanel)
            {
                onHackedEvent?.Invoke();
                a_hackedType = HackedType.ControlPanel;
                a_cameraFollow = null;
            }
            else if (objectType == HackedType.Lever)
            {
                onHackedEvent?.Invoke();
                leverUsed = true;
            
                a_hackedType = HackedType.Lever;
                a_cameraFollow = null;
                if (alreadyRotated == false)
                {
                    tempRotateForLever = true;
                    alreadyRotated = true;
                }
            }
            else if (objectType == HackedType.Wire)
            {
                a_hackedType = HackedType.Wire;
                a_cameraFollow = cameraFollow.transform;
            }
            else
            {
                a_hackedType = HackedType.INVALID;
                a_cameraFollow = null;
            }

        }

        [SerializeField] private GameObject rotateAnchor = null;
        [SerializeField] private float rotateAmount = 70;
        private bool tempRotateForLever = false;
        private float timer = 0;
        [SerializeField] private float timeForRotate = 2f;
        private bool alreadyRotated = false;
        private void Update()
        {
            if (objectType == HackedType.Lever && tempRotateForLever)
            {
                timer += Time.deltaTime;
                if (timer > timeForRotate)
                {
                    tempRotateForLever = false;
                    return;
                }
                rotateAnchor.transform.Rotate(new Vector3((rotateAmount * Time.deltaTime) / timeForRotate,0,0));
            
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
                if (indiciator != null && indiciator.activeInHierarchy)
                    indiciator.SetActive(false);
            }
        }

    }
}