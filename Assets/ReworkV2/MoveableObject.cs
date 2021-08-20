using Malicious.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Malicious.ReworkV2
{
    public class MoveableObject : MonoBehaviour, IPlayerObject

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

    public void OnHackValid()
    {
        //have material on node thing to change or play particle effect
        //to indicate the player can now hack this object
    }

    public void OnHackFalse()
    {
        //have material on node thing to change or play particle effect
        //to tell the player they can no longer hack it
    }
    
    public bool RequiresOffset() => false;
    public OffsetContainer GiveOffset()
    {
        OffsetContainer temp = new OffsetContainer();
        temp._offsetTransform = _values._cameraOffset;
        temp._rigOffset = _values._rigOffset;
        return temp;
    }
    public void SetOffset(Transform a_transform)
    {
        _values._movementTransform = a_transform;

        //This is so the camera offset is facing the same way as the player but keeping all other offset values
        float yValue = a_transform.rotation.eulerAngles.y;
        Vector3 prevEular = _values._cameraOffset.rotation.eulerAngles;
        _values._cameraOffset.rotation = Quaternion.Euler(prevEular.x, yValue, prevEular.z);
    }

    private void ExitObject(InputAction.CallbackContext a_context)
    {
        PlayerController.PlayerControl.ResetToPlayer();
    }
    private void EnableInput()
    {
        GlobalData.InputManager.Player.Enable();
        //GlobalData.InputManager.Player.Movement.performed += MoveInputEnter;
        //GlobalData.InputManager.Player.Movement.canceled += MoveInputExit;
        //GlobalData.InputManager.Player.Jump.performed += JumpInputEnter;
        //GlobalData.InputManager.Player.Jump.canceled += JumpInputExit;
        //GlobalData.InputManager.Player.Camera.performed += SpinInputEnter;
        //GlobalData.InputManager.Player.Camera.canceled += SpinInputExit;
        GlobalData.InputManager.Player.Interaction.performed += ExitObject;
    }

    private void DisableInput()
    {
        GlobalData.InputManager.Player.Disable();
        //GlobalData.InputManager.Player.Movement.performed -= MoveInputEnter;
        //GlobalData.InputManager.Player.Movement.canceled -= MoveInputExit;
        //GlobalData.InputManager.Player.Jump.performed -= JumpInputEnter;
        //GlobalData.InputManager.Player.Jump.canceled -= JumpInputExit;
        //GlobalData.InputManager.Player.Camera.performed -= SpinInputEnter;
        //GlobalData.InputManager.Player.Camera.canceled -= SpinInputExit;
        GlobalData.InputManager.Player.Interaction.performed -= ExitObject;
    }


    }
}