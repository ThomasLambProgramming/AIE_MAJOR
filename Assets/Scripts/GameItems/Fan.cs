using UnityEngine;

namespace Malicious.GameItems
{
    [SelectionBase]
    public class Fan : MonoBehaviour
    {
        private bool _isActive = true;
        [SerializeField] private GameObject _rotateObject = null;
        [SerializeField] private float _propelForce = 100f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private Vector3 _launchDirection = Vector3.up;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _isActive)
            {
                other.GetComponent<Player.Player>().FanLaunch(_launchDirection * _propelForce);
            }
        }

        private void Update()
        {
            if (_isActive)
                _rotateObject.transform.Rotate(0,_rotateSpeed * Time.deltaTime,0);
                
        }

        public void Deactivate() => _isActive = false;
        public void Activate() => _isActive = true;

    }
}
