using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    private Vector3 startingPosition = Vector3.zero;
    private bool openDoor = false;
    private bool closeDoor = true;
    private bool doneMoving = true;
    private void Start()
    {
        startingPosition = transform.position;
        targetPosition = startingPosition + targetPosition;
    }

    void FixedUpdate()
    {
        if (doneMoving)
            return;
        
        if (openDoor)
        {
            Vector3 direction = targetPosition - startingPosition;
            transform.position = transform.position + direction * (Time.deltaTime * openSpeed);
            if (Vector3.SqrMagnitude(direction) < 0.1f)
                doneMoving = true;
        }
        else
        {
            Vector3 direction = startingPosition - targetPosition;
            transform.position = transform.position + direction * (Time.deltaTime * openSpeed);
            if (Vector3.SqrMagnitude(direction) < 0.1f)
                doneMoving = true;
        }
    }
    [ContextMenu("Open Door")]
    public void Open()
    {
        openDoor = true;
        closeDoor = false;
        doneMoving = false;
    }
    [ContextMenu("Close Door")]
    public void Close()
    {
        openDoor = false;
        closeDoor = true;
        doneMoving = false;
    }
}
