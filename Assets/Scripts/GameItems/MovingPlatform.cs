using UnityEngine;

namespace Malicious.GameItems
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private float m_moveSpeed = 0.5f;
        [SerializeField] private float m_stoppingDistance = 0.4f;
        [SerializeField] private Transform m_target = null;
        private Vector3 m_targetLocation = Vector3.zero;
        private Vector3 m_startLocation = Vector3.zero;
        Vector3 m_movementAmount = Vector3.zero;

        void Start()
        {
            m_startLocation = transform.position;
            m_targetLocation = m_target.position;
            m_movementAmount = m_targetLocation - m_startLocation;
        }

        void FixedUpdate()
        {
            //later add a timer for waiting at the position for a short time and a slow down as it gets closer to the platform
            if (Vector3.SqrMagnitude(m_targetLocation - transform.position) < m_stoppingDistance)
            {
                Vector3 buffer = m_startLocation;
                m_startLocation = m_targetLocation;
                m_targetLocation = buffer;
                
                m_movementAmount = m_targetLocation - m_startLocation;
            }
            else
            {
                transform.position += m_movementAmount * (Time.deltaTime * m_moveSpeed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = this.transform;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = null;
            }
        }
    }
}
