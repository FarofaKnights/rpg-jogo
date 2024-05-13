using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerMoviment playerMoviment;

    public bool isInteracting;
    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerMoviment = GetComponent<PlayerMoviment>();
        cameraHandler = CameraHandler.singleton;

    }  
    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");  

        inputHandler.TickInput(delta);
        playerMoviment.HandleMovement(delta);
        playerMoviment.HandleRollingAndSprinting(delta);
        playerMoviment.HandleFalling(delta, playerMoviment.moveDirection);
    }
    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        isSprinting = inputHandler.b_Input;

        if (isInAir)
        {
            playerMoviment.inAirTimer = playerMoviment.inAirTimer + Time.deltaTime;
        }
    }
}
