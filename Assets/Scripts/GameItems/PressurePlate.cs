using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Malicious.Hackable;

namespace Malicious.GameItems
{
    public class PressurePlate : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private UnityEvent _OnEvent;
        [SerializeField] private UnityEvent _OffEvent;
        [SerializeField] private AudioSource _pressurePlateDownAudio = null;

        [SerializeField] private List<MeshRenderer> _cableRenderers = new List<MeshRenderer>();
        [SerializeField] private Material _onMaterial = null;
        [SerializeField] private Material _defaultMaterial = null;
        //[SerializeField] private float _horizontalCheck = 3f;

        //This is for checking that objects are still in holding it down
        private List<GameObject> _containedObjects = new List<GameObject>();
        private void OnTriggerEnter(Collider other)
        {
            //the & checks if both masks have the same bit then give a resulting number
            //made out of the bits that both share if the result has any bits similar it will
            //return greater than 0
            if (other.isTrigger)
                return; 
            
            if ((_mask & (1 << other.gameObject.layer)) > 0)
            {
                if (other.gameObject.layer == 16)
                {
                    MoveableBlock temp = other.gameObject.GetComponent<MoveableBlock>();
                    if (temp != null)
                    {
                        temp._onPressurePlate = true;
                    }
                }
                _containedObjects.Add(other.gameObject);
                if (_containedObjects.Count == 1)
                {
                    //we only want to press it down when its the first object
                    //this is all done so multiple objects can be on it without causing issues
                    //or overlaps
                    _pressurePlateDownAudio.Play();
                    _OnEvent?.Invoke();
                    Debug.Log("ONEvent");

                    if (_cableRenderers.Count > 0)
                    {
                        foreach(MeshRenderer renderer in _cableRenderers)
                        {
                            renderer.material = _onMaterial;
                        }
                    }
                }
            }
        }
        //private void Update()
        //{
        //    foreach(var item in _containedObjects)
        //    {
        //        if (Vector3.Distance(item.transform.position, transform.position) > _horizontalCheck)
        //        {
        //            if (item.layer == 16)
        //            {
        //                item.GetComponent<MoveableBlock>()._onPressurePlate = false;
        //            }
        //            if (_containedObjects.Remove(item))
        //            {

        //                if (_containedObjects.Count <= 0)
        //                {
        //                    _OffEvent?.Invoke();
        //                    Debug.Log("OffEvent");


        //                    if (_cableRenderers.Count > 0)
        //                    {
        //                        foreach (MeshRenderer renderer in _cableRenderers)
        //                        {
        //                            renderer.material = _defaultMaterial;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
                return;

            if (_containedObjects.Contains(other.gameObject))
            {
                if (other.gameObject.layer == 16)
                {
                    other.gameObject.GetComponent<MoveableBlock>()._onPressurePlate = false;
                }
                if (_containedObjects.Remove(other.gameObject))
                {

                    if (_containedObjects.Count <= 0)
                    {
                        _OffEvent?.Invoke();
                        Debug.Log("OffEvent");

                        if (_cableRenderers.Count > 0)
                        {
                            foreach (MeshRenderer renderer in _cableRenderers)
                            {
                                renderer.material = _defaultMaterial;
                            }
                        }
                    }
                }
            }
        }
        #if UNITY_EDITOR
        [ContextMenu("RUN ON EVENT")]
        public void RunON()
        {
            _OnEvent?.Invoke();
        }
        #endif
    }
}