using System;
using System.Collections;
using System.Collections.Generic;
using Malicious.Hackable;
using UnityEngine;
using Malicious.Core;

namespace Malicious.GameItems
{
    [SelectionBase]
    public class Fan : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;

        [SerializeField] private GameObject _rotateObject = null;
        [SerializeField] private float _rotateFanSpeed = 10f;
        [SerializeField] private float _rotateSpeed = 10f;
    
        [SerializeField] private float _fanHeight = 5f;
        [SerializeField] private float _horizontalDifferenceAllow = 2f;
        private float _sqrhorizontalDifferenceAllow = 2f;


        [SerializeField] private LayerMask _objectsAllowed = ~0;
        [SerializeField] private Vector3 _launchDirection = Vector3.up;

        [Header("End of fan Variables")]
        [SerializeField] private float _minPlayerForce = 3f;
        [SerializeField] private float _minBlockForce = 3f;
        [SerializeField] private float _minGroundEnemyForce = 3f;
        [SerializeField] private float _minFlyingEnemyForce = 3f;
        [SerializeField] private float _minSpringForce = 3f;
        
        [Header("Front of fan Variables")]
        [SerializeField] private float _maxPlayerForce = 10f;
        [SerializeField] private float _maxBlockForce = 10f;
        [SerializeField] private float _maxGroundEnemyForce = 10f;
        [SerializeField] private float _maxFlyingEnemyForce = 10f;
        [SerializeField] private float _maxSpringForce = 10f;

        [Header("Correction Variables")]
        [SerializeField] private float _correctionDotAllowance = 0.5f;
        
        [SerializeField] private float _correctionPlayerForce = 0.7f;
        [SerializeField] private float _correctionBlockForce = 1f;
        [SerializeField] private float _correctionGroundEnemyForce = 1f;
        [SerializeField] private float _correctionFlyingEnemyForce = 1f;
        [SerializeField] private float _correctionSpringForce = 1f;
        
        [SerializeField] private float _velocityPreservePlayer = 0.7f;
        [SerializeField] private float _velocityPreserveBlock = 1f;
        [SerializeField] private float _velocityPreserveGroundEnemy = 1f;
        [SerializeField] private float _velocityPreserveFlyingEnemy = 1f;
        [SerializeField] private float _velocityPreserveSpring = 1f;

        [Header("Velocity limit variables")]
        [SerializeField] private float _playerVelLimit = 10f;
        [SerializeField] private float _blockVelLimit = 10f;
        [SerializeField] private float _groundEnemyVelLimit = 10f;
        [SerializeField] private float _flyingEnemyVelLimit = 10f;
        [SerializeField] private float _springVelLimit = 10f;

        private List<Rigidbody> _playerList = new List<Rigidbody>();
        private List<Rigidbody> _blockList = new List<Rigidbody>();
        private List<Rigidbody> _groundEnemyList = new List<Rigidbody>();
        private List<Rigidbody> _flyingEnemyList = new List<Rigidbody>();
        private List<Rigidbody> _springList = new List<Rigidbody>();

        private void Start()
        {
            _sqrhorizontalDifferenceAllow = _horizontalDifferenceAllow * _horizontalDifferenceAllow;
            if (_launchDirection == Vector3.zero)
                _launchDirection = transform.up;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return; 
            
            Rigidbody objectRb = other.gameObject.GetComponent<Rigidbody>();

            if (objectRb != null && (_objectsAllowed & (1 << other.gameObject.layer)) > 0)
            {
                switch(other.gameObject.layer)
                {
                    case 10:
                        _playerList.Add(objectRb);
                        if (_launchDirection != Vector3.up)
                            other.gameObject.GetComponent<Player>().EnteredFan(false);
                        else
                            other.gameObject.GetComponent<Player>().EnteredFan(true);
                        break;
                    case 11:
                        _groundEnemyList.Add(objectRb);
                        SetFan(other.gameObject);
                        break;
                    case 14:
                        _flyingEnemyList.Add(objectRb);
                        SetFan(other.gameObject);
                        break;
                    case 15:
                        _springList.Add(objectRb);
                        SetFan(other.gameObject);
                        break;
                    case 16:
                        _blockList.Add(objectRb);
                        SetFan(other.gameObject);
                        break;
                }
            }
        }
        private void SetFan(GameObject a_object)
        {
            if (_launchDirection != Vector3.up)
                a_object.GetComponent<BasePlayer>().EnteredFan(false);
            else
                a_object.GetComponent<BasePlayer>().EnteredFan(true);
        }
        private void DeSetFan(GameObject a_object)
        {
            if (_launchDirection != Vector3.up)
                a_object.GetComponent<BasePlayer>().ExitedFan(false);
            else
                a_object.GetComponent<BasePlayer>().ExitedFan(true);
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
                return;
            
            if (CheckList(ref _playerList, other.gameObject))
                return;
            if (CheckList(ref _blockList, other.gameObject))
                return;
            if (CheckList(ref _groundEnemyList, other.gameObject))
                return;
            if (CheckList(ref _flyingEnemyList, other.gameObject))
                return;
            if (CheckList(ref _springList, other.gameObject))
                return;
        }
        private bool CheckList(ref List<Rigidbody> a_list, GameObject a_object)
        {
            for (int i = 0; i < a_list.Count; i++)
            {
                if (a_list[i].gameObject == a_object)
                {
                    if (a_list[i].gameObject.layer == 10)
                    {
                        if (_launchDirection != Vector3.up)
                            a_object.GetComponent<Player>().ExitedFan(false);
                        else
                            a_object.GetComponent<Player>().ExitedFan(true);
                    }
                    else
                    {
                        DeSetFan(a_object);
                    }
                    a_list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            ApplyForces(ref _playerList, _minPlayerForce, _maxPlayerForce, _playerVelLimit, _correctionPlayerForce, _velocityPreservePlayer);
            ApplyForces(ref _groundEnemyList, _minGroundEnemyForce, _maxGroundEnemyForce, _groundEnemyVelLimit, _correctionGroundEnemyForce, _velocityPreserveGroundEnemy);
            ApplyForces(ref _flyingEnemyList, _minFlyingEnemyForce, _maxFlyingEnemyForce, _flyingEnemyVelLimit, _correctionFlyingEnemyForce, _velocityPreserveFlyingEnemy);
            ApplyForces(ref _blockList, _minBlockForce, _maxBlockForce, _blockVelLimit, _correctionBlockForce, _velocityPreserveBlock);
            ApplyForces(ref _springList, _minSpringForce, _maxSpringForce, _springVelLimit, _correctionSpringForce, _velocityPreserveSpring);
        }
        private void ApplyForces(ref List<Rigidbody> a_list, float a_minForce, float a_maxForce, float a_velLimit, float a_correctionForce, float a_preserveForce)
        {
            for (int i = 0; i < a_list.Count; i++)
            {
                Vector3 difference = a_list[i].transform.position - transform.position;
                
                if (_launchDirection == Vector3.up)
                {
                    Vector2 horizontalDiff = new Vector2(difference.x, difference.z);

                    if (horizontalDiff.sqrMagnitude > _sqrhorizontalDifferenceAllow || difference.y > _fanHeight)
                    {
                        a_list.RemoveAt(i);
                        if (a_list.Count > 0)
                            i--;
                        continue;
                    }
                }

                float lerpAmount = difference.y / _fanHeight;
                if (lerpAmount < 0.1f)
                    lerpAmount = 0.1f;

                float forceScale = Mathf.Lerp(a_maxForce, a_minForce, lerpAmount);
                
                if (_launchDirection != Vector3.up)
                {
                    forceScale = Mathf.Lerp(a_maxForce, a_minForce, Vector3.SqrMagnitude(difference) / (_fanHeight * _fanHeight));
                }

                Vector3 forceToApply = _launchDirection * forceScale;
                a_list[i].AddForce(forceToApply);
                
                if (_launchDirection == Vector3.up)
                {
                    if (a_list[i].velocity.y > a_velLimit)
                    {
                        Vector3 currentVel = a_list[i].velocity;
                        currentVel.y = a_velLimit;
                        a_list[i].velocity = currentVel;
                    }
                }
                else
                {
                    Vector3 currentVel = a_list[i].velocity;
                    float currentY = currentVel.y;
                    currentVel.y = 0;

                    if (Vector3.Dot(_launchDirection, currentVel.normalized) < _correctionDotAllowance)
                    {
                        //i hate this fan, so much division and sqr roots but i cant be bothered anymore
                        //take the current length take 10% of that off, then add the launch direction equal to 10% of the original
                        float correctionForce = currentVel.magnitude;
                        currentVel *= a_preserveForce;
                        currentVel += _launchDirection * (correctionForce * a_correctionForce);
                    }
                    
                    
                    if (currentVel.sqrMagnitude > a_velLimit)
                    {
                        currentVel= currentVel.normalized * a_velLimit;
                        currentVel.y = currentY;
                        a_list[i].velocity = currentVel;
                    }
                }
            }
        }
        public void RotateFan(Vector3 a_goalRotation)
        {
            //insurance that its not overlapping rotates
            StopCoroutine(RotateFanEnumerator(Vector3.zero));
            StartCoroutine(RotateFanEnumerator(a_goalRotation));
        }
        private IEnumerator RotateFanEnumerator(Vector3 a_target)
        {
            Quaternion goalRot = Quaternion.Euler(a_target);

            while (transform.rotation != goalRot)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    goalRot, 
                    _rotateFanSpeed * Time.deltaTime);
                _launchDirection = -transform.up;
                yield return null;
            }
        }

        private void Update()
        {
            if (_isActive)
                _rotateObject.transform.Rotate(0,_rotateSpeed * Time.deltaTime,0);  
        }

        public void Deactivate() => _isActive = false;
        public void Activate() => _isActive = true;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 currentPosition = transform.position;
            currentPosition.y += _fanHeight;
            Gizmos.DrawLine(transform.position, currentPosition);
            Gizmos.DrawLine(transform.position, transform.position + _launchDirection * 4);
        }
#endif
    }
}
