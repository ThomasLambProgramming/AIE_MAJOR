using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious
{
    public class PlayerMovement
    {
        private GameObject currentPlayerObject = null;
        private Rigidbody currentRigidbody = null;
        
        PlayerMovement(GameObject a_playerObject, Rigidbody a_playerRigidbody)
        {
            currentPlayerObject = a_playerObject;
            currentRigidbody = a_playerRigidbody;
        }
        void UpdatePlayer(GameObject a_playerObject, Rigidbody a_playerRigidbody)
        {
            currentPlayerObject = a_playerObject;
            currentRigidbody = a_playerRigidbody;
        }  
        
        void StandardMove()
        {
            
        }

        void WireMove()
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
        
        void GroundEnemyMove()
        {
            
        }

        void FlyingEnemyMove()
        {
            
        }
    }
}
