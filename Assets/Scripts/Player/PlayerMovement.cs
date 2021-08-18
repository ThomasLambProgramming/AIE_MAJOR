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
            if (a_spinInput != Vector2.zero)
            {
                currentPlayerObject.transform.Rotate(new Vector3(0, a_spinInput.x * a_spinSpeed * Time.deltaTime, 0));
            }
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

        public void GroundCheck()
        {
            Collider[] collisions = Physics.OverlapSphere(groundCheck.position, 0.5f, groundMask);
            if (collisions.Length > 0)
            {
                foreach (var collider in collisions)
                {
                    if (collider.transform.CompareTag("Ground"))
                    {
                        ResetJump();
                    }
                }
            }
        }

        //needs seperate function as the move goes off camera transform not players forward direction
        public void HackObjectMove(Vector2 a_moveInput, float a_moveSpeed, Transform a_camTransform)
        {
            if (a_moveInput != Vector2.zero)
            {
                Vector3 camForward = a_camTransform.forward;
                //camForward.y = 0;

                Vector3 camRight = a_camTransform.right;
                //camRight.y = 0;
                
                float currentYAmount = currentPlayerRigidbody.velocity.y;
                Vector3 newVel =
                    camForward * (a_moveInput.y * a_moveSpeed * Time.deltaTime) +
                    camRight * (a_moveInput.x * a_moveSpeed * Time.deltaTime);
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


        #region WireVariables
        public int m_pathIndex = 0;
        public List<Vector3> m_wirePath = new List<Vector3>();
        private float m_goNextWire = 0.2f;
        private float m_wireSpeed = 10f;
        private float m_rotateSpeed = 10f;
        public Quaternion m_rotationGoal = Quaternion.identity;
        private bool m_rotateWireCam = false;
        #endregion
        public void WireMove()
        {
            if (Vector3.SqrMagnitude(currentPlayerObject.transform.position - m_wirePath[m_pathIndex]) > m_goNextWire)
            {
                Vector3 currentWirePos = currentPlayerObject.transform.position;
                currentWirePos = currentWirePos + (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized *
                    (Time.deltaTime * (m_wireSpeed));
                currentPlayerObject.transform.position = currentWirePos;
                if (m_rotateWireCam)
                {
                    currentPlayerObject.transform.rotation = Quaternion.RotateTowards(
                        currentPlayerObject.transform.rotation,
                        m_rotationGoal,
                        m_rotateSpeed);
                    if (currentPlayerObject.transform.rotation == m_rotationGoal)
                        m_rotateWireCam = false;
                }
            }
            else
            {
                if (m_pathIndex < m_wirePath.Count - 1)
                {
                    m_pathIndex++;
                    m_rotationGoal = Quaternion.LookRotation(
                        (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized, Vector3.up);
                    m_rotateWireCam = true;
                }
                else
                {
                    //end of path
                    PlayerController.PlayerControl.SwapPlayer(0);
                }
            }
        }
        
        public void GroundEnemyMove()
        {
            
        }

        public void FlyingEnemyMove()
        {
            
        }
    }
}
