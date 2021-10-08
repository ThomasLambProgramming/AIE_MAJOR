using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private float _laserDistance = 50f;

        private LineRenderer _lineRenderer = null;
        // Start is called before the first frame update
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
