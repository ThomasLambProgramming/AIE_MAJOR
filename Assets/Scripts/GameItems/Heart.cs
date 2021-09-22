using System;
using Malicious.Core;
using UnityEngine;

namespace Malicious.GameItems
{
    public class Heart : MonoBehaviour
    {
        [SerializeField] float _offsetAmount = 2f;
        [SerializeField] float _rotateSpeed = 10f;
        [SerializeField] float _moveSpeed = 10f;

        private Vector3 _initalPosition = Vector3.zero;
        private void Start()
        {
            _initalPosition = transform.position;
            GameEventManager.PlayerDead += Reset;
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime, 0));
            Vector3 newPosition = _initalPosition;
            newPosition.y = _initalPosition.y + (Mathf.Sin(Time.time * _moveSpeed) * _offsetAmount);
            transform.position = newPosition;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameEventManager.PlayerHealedFunc();
                //run particle effect or something
                gameObject.SetActive(false);
            }
        }

        private void Reset()
        {
            gameObject.SetActive(true);
        }
    }
}