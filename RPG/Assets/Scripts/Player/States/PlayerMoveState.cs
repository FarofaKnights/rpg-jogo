using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : IPlayerState {
    private float speedPlayer;
    private CharacterController characterPlayer;
    private Animator animationPlayer;
    private Vector3 movementPlayer;


    private static string verticalAxis = "inputX";
    private static string horizontalAxis = "inputZ";


    public Player player;
    Vector2 directions;

    public PlayerMoveState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        animationPlayer = player.animator;
        characterPlayer = player.GetComponent<CharacterController>();
        speedPlayer = player.moveSpeed;

        GameManager.instance.controls.Player.Movement.performed += moving  =>  directions = moving.ReadValue<Vector2>();
    }

    public void Execute() {
        HandleRotation();
        PlayerMovement();
        Gravity();
    }

    public void Exit() {
        animationPlayer.SetFloat(verticalAxis, 0);
        animationPlayer.SetFloat(horizontalAxis, 0);
    }

    private void PlayerMovement()
    {
        float horizontal = directions.x;
        float vertical = directions.y;

        movementPlayer = (player.camera.transform.right * horizontal) + (player.camera.transform.forward * vertical);
        movementPlayer.Normalize();
        movementPlayer.y = 0;

        float speed = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //animationPlayer.SetBool("Correr", true);
            speed = 2;
        }
        else
        {
            //animationPlayer.SetBool("Correr", false);
        }

        characterPlayer.Move(movementPlayer.normalized * speedPlayer * speed * Time.deltaTime);

        PlayerAnimation(horizontal, vertical);
    }

    private void HandleRotation()
    {
        float rs = player.cameraSpeed;

        Vector3 targetDir = player.camera.transform.forward;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, rs * Time.deltaTime);

        player.transform.rotation = targetRotation;
    }

    private void Gravity()
    {
        movementPlayer = Vector3.zero;
        if (characterPlayer.isGrounded == false)
        {
            movementPlayer += Physics.gravity; 
        }
        characterPlayer.Move(movementPlayer * Time.deltaTime);
    }

    private void PlayerAnimation(float x, float z)
    {
        animationPlayer.SetFloat(verticalAxis, x);
        animationPlayer.SetFloat(horizontalAxis, z);
    }
}