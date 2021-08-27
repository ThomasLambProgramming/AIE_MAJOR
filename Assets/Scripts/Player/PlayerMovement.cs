using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            groundMask = 1 << LayerMask.NameToLayer("Ground");
        }
        public void UpdatePlayer(GameObject a_playerObject, Rigidbody a_playerRigidbody)
        {
            currentPlayerObject = a_playerObject;
            currentPlayerRigidbody = a_playerRigidbody;
        }

        public void SpinMove(Vector2 a_spinInput, float a_spinSpeed)
        {
            
        }

        #region JumpVariables

        private bool canJump = true;
        private bool canDoubleJump = true;
        private Transform groundCheck = null;
        private float jumpForce = 300f;
        #endregion
        public void PlayerJump()
        {
            Debug.Log(canJump);
            Debug.Log(canDoubleJump);
            if (canJump)
            {
                canJump = false;
                currentPlayerRigidbody.AddForce(Vector3.up * jumpForce);
            }
            else if (canDoubleJump)
            {
                canDoubleJump = false;
                currentPlayerRigidbody.AddForce(Vector3.up * jumpForce);
            }
        }

        public void ResetJump()
        {
            canJump = true;
            canDoubleJump = true;
        }

        public void SetJumpVariables(Transform a_groundCheck)
        {
            groundCheck = a_groundCheck;
        }

        private int groundMask = 0;
        public void StandardMove(Vector2 a_moveInput, float a_moveSpeed)
        {
            
        }

        
    }
}
