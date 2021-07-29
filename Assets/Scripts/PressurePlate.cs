using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Door movementThing = null;
    private Door door = null;
    private void Start()
    {
        door = movementThing.GetComponent<Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HackableObject>() != null)
        {
            door.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<HackableObject>() != null)
        {
            door.Close();
        }
    }
}
