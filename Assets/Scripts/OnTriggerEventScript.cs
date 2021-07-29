using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//this makes it so the rigidbody + collider will be auto applied to the object when this is added
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class OnTriggerEventScript : MonoBehaviour
{
    [Header("This component is for trigger events to be easily added")]
    [Header("Rigidbody will be auto set to kinematic + box collider will be set to trigger in start")]

    [SerializeField] private UnityEvent onEnterEvents;
    [SerializeField] private UnityEvent onStayEvents;
    [SerializeField] private UnityEvent onExitEvents;

    
    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        onEnterEvents?.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        onStayEvents?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        onExitEvents?.Invoke();
    }
}
