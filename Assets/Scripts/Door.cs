using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 offsetPosition = Vector3.zero;
    private Vector3 startingPosition = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    
    private bool openDoor = false;
    
    private bool doneMoving = true;
    private void Start()
    {
        startingPosition = transform.position;
        offsetPosition = startingPosition + offsetPosition;
    }

    void FixedUpdate()
    {
        if (!doneMoving)
        {

            if (openDoor)
            {
                if (Vector3.SqrMagnitude(offsetPosition - transform.position) < 0.001f)
                {
                    transform.position = offsetPosition;
                    doneMoving = true;
                    return;
                }
                moveDirection = offsetPosition - startingPosition;
                moveDirection = moveDirection.normalized;
                transform.position = transform.position + moveDirection * (Time.deltaTime * openSpeed);
            }
            else
            {
                if (Vector3.SqrMagnitude(transform.position - startingPosition) < 0.001f)
                {
                    transform.position = startingPosition;
                    doneMoving = true;
                    return;   
                }
                moveDirection = startingPosition - offsetPosition;
                moveDirection = moveDirection.normalized;
                transform.position = transform.position + moveDirection * (Time.deltaTime * openSpeed);
            }
        }
    }
    [ContextMenu("Open Door")]
    public void Open()
    {
        openDoor = true;
        
        doneMoving = false;
    }
    [ContextMenu("Close Door")]
    public void Close()
    {
        openDoor = false;
        
        doneMoving = false;
    }
}
