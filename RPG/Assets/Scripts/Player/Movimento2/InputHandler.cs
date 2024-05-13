using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmout;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool lockOnInput;
    public bool right_Stick_Right_Input;
    public bool right_Stic_Left_Input;

    public bool rollFlag;
    public bool sprintFlag;
    public float rollInputTimer;
    public bool LockOnFlag;

    PlayerControls inputActions;
    CameraHandler cameraHandler;
    

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        cameraHandler = FindAnyObjectByType<CameraHandler>();
    }

    public void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerAction.lockOnInput.performed += i => lockOnInput = true;
            inputActions.PlayerMovement.LockonTargertRight.performed += i => right_Stick_Right_Input = true;
            inputActions.PlayerMovement.LockOnTargertLeft.performed += i => right_Stic_Left_Input = true;

        }
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
        HandleLockOnInput();
    }

    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmout = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {
        b_Input = inputActions.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed; 
        if (b_Input)
        {
            rollInputTimer += delta;
            sprintFlag = true;
        }
        else
        {
            if(rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }
    }

    private void HandleLockOnInput()
    {
        if(lockOnInput && LockOnFlag == false)
        {
            lockOnInput = false;
            cameraHandler.HandleLockOn();
            if(cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                LockOnFlag = true;
            }
        }
        else if(lockOnInput && LockOnFlag)
        {
            lockOnInput = false;
            LockOnFlag = false;
            cameraHandler.ClearLockTargets();
        }

        if(LockOnFlag && right_Stic_Left_Input)
        {
            right_Stic_Left_Input = false;
            cameraHandler.HandleLockOn();
            if(cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
            }
        }
        if(LockOnFlag && right_Stick_Right_Input)
        {
            right_Stick_Right_Input = false;
            cameraHandler.HandleLockOn();
            if(cameraHandler.RightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.RightLockTarget;
            }
        }
    }
}
