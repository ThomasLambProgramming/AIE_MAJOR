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
        if (_values._rigidbody.isKinematic)
            _values._rigidbody.isKinematic = false;

    }

    public void OnHackExit()
    {
        DisableInput();
        if (_values._rigidbody.isKinematic == false)
            _values._rigidbody.isKinematic = true;
    }

    public void Tick()
    {
        //Logic
        SpinMovement();
    }

    public void FixedTick()
    {
        //Movement and etc
        Movement();
    }

    private void Movement()
    {
        if (_values._moveInput != Vector2.zero)
        {
            Vector3 camForward = _values._cameraOffset.forward;
            //camForward.y = 0;

            Vector3 camRight = _values._cameraOffset.right;
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
        if (_values._spinInput != Vector2.zero)
        {
            _values._cameraOffset.RotateAround(_values._cameraOffset.position, Vector3.up,
                _values._spinInput.x * _values._spinSpeed * Time.deltaTime);
        }
    }

    
    private void SpinInputStart(InputAction.CallbackContext a_context)
    {
        _values._spinInput = a_context.ReadValue<Vector2>();
    }
    private void SpinInputEnd(InputAction.CallbackContext a_context)
    {
        _values._spinInput = Vector2.zero;
    }
    private void MoveInputStart(InputAction.CallbackContext a_context)
    {
        _values._moveInput = a_context.ReadValue<Vector2>();
    }
    private void MoveInputEnd(InputAction.CallbackContext a_context)
    {
        _values._moveInput = Vector2.zero;
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
    public bool RequiresTruePlayerOffset() => true;
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
        
        //Place the player half way between the moveable object and the camera offset
        //this will need edge cases to make sure player cannot be placed out of bounds
        
        PlayerController.PlayerControl.ResetToPlayer(
            new Vector3(transform.position.x, 
                transform.position.y + 2, 
                transform.position.z), 
            _values._cameraOffset.rotation);
    }
    private void EnableInput()                          
    {
        GlobalData.InputManager.Player.Enable();
        GlobalData.InputManager.Player.Movement.performed += MoveInputStart;
        GlobalData.InputManager.Player.Movement.canceled += MoveInputEnd;
        GlobalData.InputManager.Player.Camera.performed += SpinInputStart;
        GlobalData.InputManager.Player.Camera.canceled += SpinInputEnd;
        GlobalData.InputManager.Player.Interaction.performed += ExitObject;
    }

    private void DisableInput()
    {
        GlobalData.InputManager.Player.Disable();
        GlobalData.InputManager.Player.Movement.performed -= MoveInputStart;
        GlobalData.InputManager.Player.Movement.canceled -= MoveInputEnd;
        GlobalData.InputManager.Player.Camera.performed -= SpinInputStart;
        GlobalData.InputManager.Player.Camera.canceled -= SpinInputEnd;
        GlobalData.InputManager.Player.Interaction.performed -= ExitObject;
    }


    }
}