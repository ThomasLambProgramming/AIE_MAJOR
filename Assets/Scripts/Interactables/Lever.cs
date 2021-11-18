using Malicious.Core;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Malicious.Interactables
{
    [SelectionBase]
    public class Lever : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool reusable = true;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private UnityEvent offEvent;

        [SerializeField] private float timer = 0;
        [SerializeField] private float timeForRotate = 2f;
        [SerializeField] private float rotateAmount = 70;
        [SerializeField] private Transform rotateAnchor = null;
        [SerializeField] private AudioSource _leverActivateSound = null;

        [SerializeField] private List<MeshRenderer> _cableRenderers = new List<MeshRenderer>();
        [SerializeField] private Material _onMaterial = null;
        [SerializeField] private Material _defaultMaterial = null;


        //private bool _beenHacked = false;
        public void OnHackValid()
        {
        }
        public void OnHackFalse()
        {
        }
        public float HackedHoldTime() => 0f;
        public bool HasHoldInput() => false;
        public void HoldInputActivate() { }

#if UNITY_EDITOR
        [SerializeField] private bool testEvents = false;
        private void Update()
        {
            if (testEvents)
            {
                Hacked();
                testEvents = false;
            }
        }
#endif

        private bool isOn = false;
        private bool isRotating = false;

        public void Hacked()
        {

            //if its in the middle of rotating dont let the player change (to not
            //cause errors with rotation)
            if (isRotating)
                return;

            if (!isOn)
            {
                _leverActivateSound.Play();
                onEvent?.Invoke();
                isOn = true;
                isRotating = true;
                timer = 0;
                GameEventManager.GeneralUpdate += Rotating;
                if (_cableRenderers.Count > 0)
                {
                    foreach (MeshRenderer renderer in _cableRenderers)
                    {
                        renderer.material = _onMaterial;
                    }
                }
            }
            else
            {
                if (reusable)
                {
                    _leverActivateSound.Play();
                    offEvent?.Invoke();
                    isOn = false;
                    timer = 0;
                    isRotating = true;
                    GameEventManager.GeneralUpdate += Rotating;
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


        float _initialAmount = 0f;
        //This is temp to make the lever rotate from one side to another
        //just for visuals (get the designers to make an animation for this
        public void Rotating()
        {

            timer += Time.deltaTime;
            if (timer > timeForRotate)
            {
                GameEventManager.GeneralUpdate -= Rotating;
                isRotating = false;

                return;
            }

            if (!isOn)
            {

                rotateAnchor.Rotate(rotateAmount * Time.deltaTime / timeForRotate, 0, 0);
            }
            else
            {

                rotateAnchor.Rotate(-rotateAmount * Time.deltaTime / timeForRotate, 0, 0);
            }
        }
    }
}
