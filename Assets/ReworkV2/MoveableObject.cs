using UnityEngine;

namespace Malicious.ReworkV2
{
    public class MoveableObject : MonoBehaviour

    {
    [SerializeField] private MoveObjValues _values = new MoveObjValues();

    private void Start()
    {
        _values._rigidbody = GetComponent<Rigidbody>();
    }

    public void OnHackEnter()
    {
        EnableInput();
    }

    public void OnHackExit()
    {
        DisableInput();
    }

    public void Tick()
    {
        //Logic
    }

    public void FixedTick()
    {
        //Movement and etc
    }

    private void Movement()
    {
        if (_values._moveInput != Vector2.zero)
        {
            Vector3 camForward = _values._movementTransform.forward;
            //camForward.y = 0;

            Vector3 camRight = _values._movementTransform.right;
            //camRight.y = 0;
                
            float currentYAmount = _values._rigidbody.velocity.y;
            Vector3 newVel =
                camForward * (_values._moveInput.y * _values._moveSpeed * Time.deltaTime) +
                camRight * (_values._moveInput.x * _values._moveSpeed * Time.deltaTime);
            newVel.y = currentYAmount;
            _values._rigidbody.velocity = newVel;
        }
            
        if (Mathf.Abs(_values._moveInput.magnitude) < 0.1f)
        {
            //if we are actually moving 
            if (Mathf.Abs(_values._rigidbody.velocity.x) > 0.2f || Mathf.Abs(_values._rigidbody.velocity.z) > 0.2f)
            {
                Vector3 newVel = _values._rigidbody.velocity;
                //takes off 5% of the current vel every physics update so the player can land on a platform without overshooting
                //because the velocity doesnt stop
                newVel.z = newVel.z * 0.95f;
                newVel.x = newVel.x * 0.95f;
                _values._rigidbody.velocity = newVel;
            }
        }
    }

    private void SpinMovement()
    {
        
    }

    public Transform GiveOffset()
    {
        throw new System.NotImplementedException();
    }

    public void SetOffset(Transform a_transform)
    {
        _values._movementTransform = a_transform;
    }
    private void EnableInput()
    {
        GlobalData.InputManager.Player.Enable();
    }

    private void DisableInput()
    {
        GlobalData.InputManager.Player.Disable();
    }


    }
}