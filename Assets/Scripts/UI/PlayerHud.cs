using System;
using UnityEngine;
using Malicious.Core;


namespace Malicious.UI
{
    public class PlayerHud : MonoBehaviour
    {
        [SerializeField] private GameObject _health1 = null;
        [SerializeField] private GameObject _health2 = null;
        [SerializeField] private GameObject _health3 = null;
        private int _currenthealth = 3;

        public void Start()
        {
            GameEventManager.PlayerHealed += AddHealth;
            GameEventManager.PlayerHit += RemoveHealth;
            GameEventManager.PlayerDead += ResetHealth;
        }

        public void RemoveHealth()
        {
            _currenthealth--;
            switch (_currenthealth)
            {
                case 2:
                    _health3.SetActive(false);
                    break;
                case 1:
                    _health2.SetActive(false);
                    break;
                case 0:
                    _health1.SetActive(false);
                    break;
            }
        }

        public void AddHealth()
        {
            _currenthealth++;
            switch (_currenthealth)
            {
                case 3:
                    _health3.SetActive(true);
                    break;
                case 2:
                    _health2.SetActive(true);
                    break;
                case 1:
                    _health1.SetActive(true);
                    break;
            }
        }

        private void ResetHealth()
        {
            _health1.SetActive(true);
            _health2.SetActive(true);
            _health3.SetActive(true);
            _currenthealth = 3;
        }
    }
}
