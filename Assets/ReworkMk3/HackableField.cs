using System;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.ReworkMk3
{
    /// <summary>
    /// Hackable field is to be used to be used for confirming that the player can enter into the
    /// hackable object
    /// </summary>
    public class HackableField : MonoBehaviour
    {
        //Other Variables//
        [SerializeField] private float _dotAllowance = 0.8f;
        private BasePlayer _hackable = null;
        private Player _player = null;
        private bool _hackValid = false;
        
        //This is to allow for sphere colliders or box colliders as needed
        [SerializeField] private Collider _triggerVolume;
        
        //Material Variables//
        [SerializeField] private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        //-------------------------------------//

        [SerializeField] private UnityEvent _onHackValidEvent = null;
        [SerializeField] private UnityEvent _onHackFalseEvent = null;

        
        [Tooltip("Only use if there is a option for holding and tapping")]
        [SerializeField] private bool _hasHoldOption = false;
        [SerializeField] private float _holdTime = 2f;
        private void Start()
        {
            _hackable = GetComponent<BasePlayer>();
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
        }


        private bool DotCheck()
        {
            Transform playerTransform = _player.transform;
            Vector3 direction = (transform.position - playerTransform.position).normalized;
            if (Vector3.Dot(direction, playerTransform.forward) > _dotAllowance)
            {
                return true;
            }

            return false;
        }

        public bool HackIntoObject()
        {
            if (_hackValid)
            {
                //run hack interface
                _hackable._player = _player;
                _hackable.OnHackEnter();
                _player = null;
                return true;
            }

            return false;
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
                OnHackFalse();
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