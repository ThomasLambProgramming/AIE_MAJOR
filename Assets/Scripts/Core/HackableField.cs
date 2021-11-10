using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Malicious.Hackable;
using Malicious.Interactables;
using System.Collections.Generic;

namespace Malicious.Core
{
    /// <summary>
    /// Hackable field is to be used to be used for confirming that the player can enter into the
    /// hackable object
    /// </summary>
    public class HackableField : MonoBehaviour
    {
        //Other Variables//
        [SerializeField] private float _dotAllowance = 0.8f;
        [SerializeField] private float _maxDistanceAway = 5f;
        [SerializeField] private float _maxTapHoldLength = 0.4f;
        private float _sqrMaxDistanceAway = 0;
        private bool _holdingHackButton = false;
        private float _holdTime = 0;
        [SerializeField] private bool _requiresSameYHeight = false;
        [SerializeField] private float _yDifferenceAllowed = 1f;
        private BasePlayer _hackable = null;
        private IInteractable _interactable = null;
        public bool _isHacked = false;
        private Player _player = null;
        private Rigidbody _playerRigid = null;
        private bool _hackValid = false;

        [SerializeField] private Transform _lookGoal = null;
        
        //This is to allow for sphere colliders or box colliders as needed

        //Material Variables//
        [SerializeField] private List<MeshRenderer> _nodeRenderer = new List<MeshRenderer>();
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        //-------------------------------------//

        [SerializeField] private UnityEvent _onHackValidEvent = null;
        [SerializeField] private UnityEvent _onHackFalseEvent = null;
        
        private void Start()
        {
            _hackable = GetComponent<BasePlayer>();
            _interactable = GetComponent<IInteractable>();
            
        }

        public void OnHackValid()
        {
            _hackValid = true;
            _onHackValidEvent?.Invoke();
            foreach (var node in _nodeRenderer)
            {
                node.material = _hackValidMaterial;
            }
        }

        public void OnHackFalse()
        {
            _hackValid = false;
            _onHackFalseEvent?.Invoke();
            foreach (var node in _nodeRenderer)
            {
                node.material = _defaultMaterial;
            }
            _holdTime = 0;
            _holdingHackButton = false;
        }


        private bool DotCheck()
        {
            Transform playerTransform = _player.transform;

            if (_requiresSameYHeight)
            {
                if (_lookGoal != null)
                {
                    if (Mathf.Abs(_lookGoal.position.y - playerTransform.position.y) > _yDifferenceAllowed)
                    {
                        return false;
                    }
                }
                else if (Mathf.Abs(transform.position.y - playerTransform.position.y) > _yDifferenceAllowed)
                {
                    return false;
                }
            }
            Vector3 direction = Vector3.zero;
            if (_lookGoal != null)
                direction = (_lookGoal.position - playerTransform.position).normalized;
            else
                direction = (transform.position - playerTransform.position).normalized;

            //This is to remove the y from the looking direction so its only if the player is looking horizontally in
            Vector2 horizontalDirection = new Vector2(direction.x, direction.z);
            horizontalDirection = horizontalDirection.normalized;
            Vector2 playerLookDirection = new Vector2(playerTransform.forward.x, playerTransform.forward.z);
            playerLookDirection = playerLookDirection.normalized;
            
            if (Vector2.Dot(horizontalDirection, playerLookDirection) > _dotAllowance)
            {
                _sqrMaxDistanceAway = _maxDistanceAway * _maxDistanceAway;
                if (Vector3.SqrMagnitude(transform.position - _player.transform.position) < _sqrMaxDistanceAway)
                    return true;
            }

            return false;
        }

        public void HackInputStarted()
        {
            if (_hackValid)
            {
                _holdingHackButton = true;
                StartCoroutine(HoldCheck());
            }
        }
        public void HackInputStopped()
        {
            _holdingHackButton = false;
            if (_holdTime >= _maxTapHoldLength)
            {
                _holdTime = 0;
                return;
            }
            if (_hackValid)
            {
                if (_interactable != null)
                {
                    _interactable.Hacked();
                }

                if (_hackable != null)
                {
                    _hackable._player = _player;
                    _hackable.OnHackEnter();
                    foreach (var node in _nodeRenderer)
                    {
                        node.material = _hackedMaterial;
                    }
                    _player.OnHackExit();
                    _player = null;
                }
            }
        }

        private IEnumerator HoldCheck()
        {
            while (_holdingHackButton)
            {
                _holdTime += Time.deltaTime;

                if (_interactable != null)
                {
                    if (_holdTime >= _interactable.HackedHoldTime())
                    {
                        _interactable.HoldInputActivate();
                        _holdingHackButton = false;
                    }
                }
                else if (_hackable != null && _hackable._hasHoldOption)
                {
                    if (_holdTime >= _hackable._holdChargeTime)
                    {
                        _hackable.HoldOptionActivate();
                        _holdingHackButton = false;
                    }
                }
                yield return null;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.gameObject.CompareTag("Player"))
            {
                _player = other.gameObject.GetComponent<Player>();
                _playerRigid = other.attachedRigidbody;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_player == null || other.isTrigger)
                return;
            
            _player.SetHackableField(this); 
            
            if (DotCheck())
                OnHackValid();
            else
            {
                OnHackFalse();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerRigid != null && other.attachedRigidbody == _playerRigid)
            {
                //There were alot of added nulls that are needed
                //as the player gets setactived alot
                if (_player != null)
                    _player.SetHackableField(null);
                
                _playerRigid = null;

                OnHackFalse();
                _player = null;
            }
        }
    }
}