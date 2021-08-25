using Malicious.Player;
using UnityEngine;
using System.Collections.Generic;

namespace Malicious.ReworkV2
{
    public class Wire : MonoBehaviour, IPlayerObject
    {
        [SerializeField] private WireValues _values = new WireValues();
        public void OnHackEnter()
        {
            _values._wireModel = PlayerController.PlayerControl._wireModel;
            _values._wireCameraOffset = PlayerController.PlayerControl._wireModelOffset;
            
            Vector3 directionToNode = (_values._wirePath[1] - _values._wirePath[0]).normalized;
            Quaternion newRotation = Quaternion.LookRotation(directionToNode);
            //currentPlayerObject.transform.rotation = newRotation;
            //currentPlayerObject.transform.position = m_wirePath[0];
        }

        public void OnHackExit()
        {
            //m_pathIndex = 0;
            //m_rotationGoal = Quaternion.identity;
        }

        public void Tick()
        {
            //if (Vector3.SqrMagnitude(currentPlayerObject.transform.position - m_wirePath[m_pathIndex]) > m_goNextWire)
            //{
            //    Vector3 currentWirePos = currentPlayerObject.transform.position;
            //    currentWirePos = currentWirePos + (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized *
            //        (Time.deltaTime * (m_wireSpeed));
            //    currentPlayerObject.transform.position = currentWirePos;
            //    if (m_rotateWireCam)
            //    {
            //        currentPlayerObject.transform.rotation = Quaternion.RotateTowards(
            //            currentPlayerObject.transform.rotation,
            //            m_rotationGoal,
            //            m_rotateSpeed);
            //        if (currentPlayerObject.transform.rotation == m_rotationGoal)
            //            m_rotateWireCam = false;
            //    }
            //}
            //else
            //{
            //    if (m_pathIndex < m_wirePath.Count - 1)
            //    {
            //        // m_pathIndex++;
            //        // m_rotationGoal = Quaternion.LookRotation(
            //        //     (m_wirePath[m_pathIndex] - currentPlayerObject.transform.position).normalized, Vector3.up);
            //        // m_rotateWireCam = true;
            //    }
            //    else
            //    {
            //        //end of path
            //        
            //    }
            //}
        }

        public void FixedTick()
        {
            throw new System.NotImplementedException();
        }

        public OffsetContainer GiveOffset()
        {
            throw new System.NotImplementedException();
        }

        public bool RequiresOffset()
        {
            throw new System.NotImplementedException();
        }
        

        public void SetOffset(Transform a_offset)
        {
            throw new System.NotImplementedException();
        }

        public void OnHackValid()
        {
            throw new System.NotImplementedException();
        }

        public void OnHackFalse()
        {
            throw new System.NotImplementedException();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_values._showPath)
            {
                foreach (var point in _values._wirePath)
                {
                    Gizmos.DrawSphere(point, 1);
                }
            }
        }
        //Add a path onto the end just makes it easier to make a path not needing to copy paste 
        //positions
        [ContextMenu("AddPathPoint")]
        public void AddPathPoint()
        {
            if (_values._wirePath.Count > 0)
            {
                Vector3 newPoint = _values._wirePath[_values._wirePath.Count - 1];
                _values._wirePath.Add(newPoint);
            }
            else
            {
                _values._wirePath = new List<Vector3>();
                _values._wirePath.Add(gameObject.transform.position);
            }
        }
#endif
    }
}