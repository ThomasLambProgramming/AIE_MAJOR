using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Malicious.Interfaces;
using UnityEngine;

namespace Malicious.Player
{
    /// <summary>
    /// This class is to separate variables, so the player class can be divided into multiple smaller classes
    /// for readability without needing to have multiple references to the same information
    /// Its monobehaviour just so its easier to add and see in inspector
    /// </summary>
    public class PlayerVariables : MonoBehaviour
    {
        //Singleton player variables to avoid memory overlap
        public static PlayerVariables m_playerVariables;
        
            
        //------Camera Variables-------------------//
        public CinemachineVirtualCamera m_MainCam = null;
            //Universal camera offset              
        public Transform m_CameraOffset = null;
            //This is the default for the player that the offset will always be based off
        [HideInInspector] public Transform m_TrueCameraOffset = null;
        
        
        //------Player Object Variables------------//
        public GameObject m_TruePlayerObject = null;
        [HideInInspector] public Rigidbody m_TrueRigidbody = null;
        [HideInInspector] public GameObject m_CurrentPlayerObject = null;
        [HideInInspector] public Rigidbody m_CurrentRigidbody = null;


        //------Hackable Variables-----------------//
        public IHackable m_currentInteractable = null;          
        
        
        //------True Player Variables--------------//
        public float m_MoveSpeed = 5f;
        public float m_JumpForce = 10f;
        public float m_SpinSpeed = 5f;


        //------Jumping Variables--------------//
        [HideInInspector] public bool m_CanJump = true;
        [HideInInspector] public bool m_HasDoubleJumped = false;
        [HideInInspector] public bool m_HoldingJump = false;
        public Transform m_GroundCheck = null;


        //------Wire Variables-----------------//
        public GameObject m_WireModel = null;
        public float m_WireSpeed = 10f;
        [HideInInspector] public bool m_InWire = false;
        [HideInInspector] public int m_PathIndex = 0;
        [HideInInspector] public float m_GoNextWire = 0.2f;
        [HideInInspector] public List<Vector3> m_WirePath = null;
        [HideInInspector] public Quaternion m_RotationGoal = Quaternion.identity;
        [HideInInspector] public float m_RotateSpeed = 10f;
        
        
        //------Player Input variables---------//
        public MasterInput m_PlayerInput = null;
        [HideInInspector] public Vector2 m_PlayerMoveInput = Vector2.zero;
        [HideInInspector] public Vector2 m_PlayerSpinInput = Vector2.zero;

        
        //------Animator Variables-------------//
        public float m_AnimationSwapSpeed = 3f;
        public Animator m_PlayerAnimator = null;
        public Vector2 m_CurrentAnimationVector = Vector2.zero;
        public readonly int m_XPos = Animator.StringToHash("XPos");
        public readonly int m_YPos = Animator.StringToHash("YPos");
        public readonly int m_Jumping = Animator.StringToHash("Jumping");
        
        void Awake()
        {
            m_playerVariables = this;
            m_PlayerInput = new MasterInput();
        }
        public void Start()
        {
            m_CurrentPlayerObject = m_TruePlayerObject;
            m_CurrentRigidbody = m_TrueRigidbody;
            m_PlayerAnimator = m_CurrentPlayerObject.GetComponent<Animator>();
        }
    }
}