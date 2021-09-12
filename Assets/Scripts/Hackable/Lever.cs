using Malicious.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Malicious.Hackable
{
    [SelectionBase]
    public class Lever : MonoBehaviour
    {
        [SerializeField] private bool reusable = true;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private UnityEvent offEvent;

        [SerializeField] private float timer = 0;
        [SerializeField] private float timeForRotate = 2f;
        [SerializeField] private float rotateAmount = 70;
        [SerializeField] private Transform rotateAnchor = null;

        [SerializeField] private GameObject _nodeObject = null;
        private MeshRenderer _nodeRenderer = null;
        [SerializeField] private Material _defaultMaterial = null;
        [SerializeField] private Material _hackValidMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;
        private bool _beenHacked = false;
        public void OnHackValid()
        { 
            if (!_beenHacked)
                _nodeRenderer.material = _hackValidMaterial;
        }

        public void OnHackFalse()
        {
            if (!_beenHacked)
                _nodeRenderer.material = _defaultMaterial;
        }
        private void Start()
        {
            _nodeRenderer = _nodeObject.GetComponent<MeshRenderer>();
        }

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
            _nodeRenderer.material = _hackedMaterial;
            _beenHacked = true;
            //if its in the middle of rotating dont let the player change (to not
            //cause errors with rotation)
            if (isRotating)
                return;
            if (!isOn)
            {
                onEvent?.Invoke();
                isOn = true;
                isRotating = true;
                timer = 0;
                GameEventManager.GeneralUpdate += Rotating;
            }
            else
            {
                if (reusable)
                {
                    offEvent?.Invoke();
                    isOn = false;
                    timer = 0;
                    isRotating = true;
                    GameEventManager.GeneralUpdate += Rotating;

                }
            }
        }


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
                rotateAnchor.Rotate(new Vector3((rotateAmount * Time.deltaTime) / timeForRotate, 0, 0));
            else
                rotateAnchor.Rotate(new Vector3((-rotateAmount * Time.deltaTime) / timeForRotate, 0, 0));
        }
    }
}
