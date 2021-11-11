using UnityEngine;
namespace Malicious.GameItems
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float _openSpeed = 3f;
        [SerializeField] private Vector3 _targetPosition = Vector3.zero;
        [SerializeField] private int _coresNeededToOpen = 0;
        [SerializeField] private AudioSource _openingAudio = null;
        private Vector3 _startingPosition = Vector3.zero;
        private float _timer = 0;
        private bool _wait = true;
        private int _coresHacked = 0;
        
        private bool _openDoor = false;
        
        private void Start()
        {
            _startingPosition = transform.position;
            _targetPosition = _startingPosition + _targetPosition;
        }

        void FixedUpdate()
        {
            if (_wait)
                return;

            if (_openDoor)
            {
                _timer += Time.deltaTime * _openSpeed;
                transform.position = Vector3.Lerp(_startingPosition, _targetPosition, _timer);

                if (_timer >= 1)
                {
                    _wait = true;
                    _timer = 1;
                }
            }
            else
            {
                _timer -= Time.deltaTime * _openSpeed;
                transform.position = Vector3.Lerp(_startingPosition, _targetPosition, _timer);

                if (_timer <= 0)
                {
                    _wait = true;
                    _timer = 0;
                }
            }
            
        }

        [ContextMenu("Open Door")]
        public void Open()
        {
            _openDoor = true;
            _wait = false;
            _openingAudio.Play();
        }
        [ContextMenu("Close Door")]
        public void Close()
        {
            _openingAudio.Play();
            _openDoor = false;
            _wait = false;
        }

        public void CoreHacked()
        {
            _coresHacked++;
            if (_coresHacked >= _coresNeededToOpen)
            {
                Open();
            }
        }
    }
}
