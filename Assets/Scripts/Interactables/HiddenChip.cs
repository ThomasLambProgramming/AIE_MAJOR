using System;
using UnityEngine;
using Malicious.UI;

namespace Malicious.Interactables
{
    class HiddenChip : MonoBehaviour
    {
        [SerializeField] int _logNumber = 0;
        

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                VoiceText._voiceText.DisplayText(_logNumber);
                Destroy(this.gameObject);
            }
        }

        [SerializeField] float _offsetAmount = 2f;
        [SerializeField] float _rotateSpeed = 10f;
        [SerializeField] float _moveSpeed = 10f;

        private Vector3 _initalPosition = Vector3.zero;
        private void Start()
        {
            _initalPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime, 0));
            Vector3 newPosition = _initalPosition;
            newPosition.y = _initalPosition.y + (Mathf.Sin(Time.time * _moveSpeed) * _offsetAmount);
            transform.position = newPosition;
        }
    }
}
