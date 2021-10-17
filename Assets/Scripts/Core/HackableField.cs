using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Malicious.Hackable;
using Malicious.Interactables;

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
        private bool _holdingHackButton = false;
        private float _holdTime = 0;
        
        private BasePlayer _hackable = null;
        private IInteractable _interactable = null;
        
        private Player _player = null;
        private bool _hackValid = false;

        [SerializeField] private Transform _lookGoal = null;
        
        //This is to allow for sphere colliders or box colliders as needed

        //Material Variables//
        [SerializeField] private MeshRenderer _nodeRenderer = null;
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
            _nodeRenderer.material = _hackValidMaterial;
        }

        public void OnHackFalse()
        {
            _hackValid = false;
            _onHackFalseEvent?.Invoke();
            _nodeRenderer.material = _defaultMaterial;
            _holdTime = 0;
            _holdingHackButton = false;
        }


        private bool DotCheck()
        {
            Transform playerTransform = _player.transform;
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
                if (Vector3.SqrMagnitude(transform.position - _player.transform.position) < _maxDistanceAway)
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
                    _nodeRenderer.material = _hackedMaterial;
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
            if (other.gameObject.CompareTag("Player"))
                _player = other.gameObject.GetComponent<Player>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_player == null)
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
            //There were alot of added nulls that are needed
            //as the player gets setactived alot
            if (_player != null)
                _player.SetHackableField(null);
            
            OnHackFalse();
            _player = null;
        }
    }
}