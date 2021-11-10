
using UnityEngine;
using System.Collections.Generic;

namespace Malicious.GameItems
{
    class Gpu : MonoBehaviour
    {
        [SerializeField] private float _spinSpeed = 400;
        [SerializeField] private List<GameObject> _fans = new List<GameObject>();

        private void Update()
        {
            foreach(var fan in _fans)
            {
                fan.transform.Rotate(0, _spinSpeed * Time.deltaTime, 0);
            }
        }
    }
}
