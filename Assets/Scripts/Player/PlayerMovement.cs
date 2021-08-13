using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.Player
{
    /// <summary>
    /// This script is to seperate all movement types out to be their own function
    /// </summary>
    public class PlayerMovement
    {
        private GameObject currentPlayerObject = null;
        private Rigidbody currentPlayerRigidbody = null;
        
        public PlayerMovement(GameObject a_playerObject, Rigidbody a_playerRigidbody)
        {
            currentPlayerObject = a_playerObject;
            currentPlayerRigidbody = a_playerRigidbody;
        }
        public void UpdatePlayer(GameObject a_playerObject, Rigidbody a_playerRigidbody)
        {
            currentPlayerObject = a_playerObject;
            currentPlayerRigidbody = a_playerRigidbody;
        }

        public void SpinMove(Vector2 a_spinInput, float a_spinSpeed)
        {
            if (a_spinInput != Vector2.zero)
            {
                currentPlayerObject.transform.Rotate(new Vector3(0, a_spinInput.x * a_spinSpeed * Time.deltaTime, 0));
            }
        }
        
        public void StandardMove(Vector2 a_moveInput, float a_moveSpeed)
        {
            if (a_moveInput != Vector2.zero)
            {
                float currentYAmount = currentPlayerRigidbody.velocity.y;
                Vector3 newVel =
                    currentPlayerObject.transform.forward * (a_moveInput.y * a_moveSpeed * Time.deltaTime) +
                    currentPlayerObject.transform.right * (a_moveInput.x * a_moveSpeed * Time.deltaTime);
                newVel.y = currentYAmount;
                currentPlayerRigidbody.velocity = newVel;
            }
            
            if (Mathf.Abs(a_moveInput.magnitude) < 0.1f)
            {
                //if we are actually moving 
                if (Mathf.Abs(currentPlayerRigidbody.velocity.x) > 0.2f || Mathf.Abs(currentPlayerRigidbody.velocity.z) > 0.2f)
                {
                    Vector3 newVel = currentPlayerRigidbody.velocity;
                    //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                    //because the velocity doesnt stop
                    newVel.z = newVel.z * 0.95f;
                    newVel.x = newVel.x * 0.95f;
                    currentPlayerRigidbody.velocity = newVel;
                }
            }
        }

        public void HackObjectMove()
        {
            
        }

        public void WireMove()
        {
            //if (Vector3.Distance(wireDummy.transform.position, wirePath[pathIndex]) > goNextDistance)
            //{
            //    Vector3 currentWirePos = wireDummy.transform.position;
            //    currentWirePos = currentWirePos + (wirePath[pathIndex] - wireDummy.transform.position).normalized *
            //        (Time.deltaTime * (wireSpeed));
            //    wireDummy.transform.position = currentWirePos;
            //    if (rotateWireCam)
            //    {
            //        wireDummy.transform.rotation = Quaternion.RotateTowards(
            //            wireDummy.transform.rotation,
            //            rotationGoalWire,
            //            rotationSpeed);
            //        if (wireDummy.transform.rotation == rotationGoalWire)
            //            rotateWireCam = false;
            //    }
            //}
            //else
            //{
            //    if (pathIndex < wirePath.Count - 1)
            //    {
            //        pathIndex++;
            //        rotationGoalWire = Quaternion.LookRotation(
            //            (wirePath[pathIndex] - wireDummy.transform.position).normalized, Vector3.up);
            //        rotateWireCam = true;
            //    }
            //    else
            //    {
            //        //end of path
            //        ResetToTruePlayer(wireDummy.transform.position);
            //        pathIndex = 0;
            //        wirePath = null;
            //        inWire = false;
            //        wireDummy.SetActive(false);
            //        mainCam.Follow = currentPlayer.transform;
            //        truePlayerObject.transform.position = wireDummy.transform.position;
            //        GameEventManager.PlayerFixedUpdate -= inWireUpdate;
            //    }
            //}
        }
        
        public void GroundEnemyMove()
        {
            
        }

        public void FlyingEnemyMove()
        {
            
        }
    }
}
