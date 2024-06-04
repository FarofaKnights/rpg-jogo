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

        Debug.Log(directions);

        movementPlayer = (player.transform.right * horizontal) + (player.transform.forward * vertical);      
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //animationPlayer.SetBool("Correr", true);
            movementPlayer = (player.transform.right * horizontal) + (player.transform.forward * vertical) * 2;
        }
        else
        {
            //animationPlayer.SetBool("Correr", false);
        }

        characterPlayer.Move(movementPlayer.normalized * speedPlayer * Time.deltaTime);

        PlayerAnimation(horizontal, vertical);
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