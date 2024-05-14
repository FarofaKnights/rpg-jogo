using UnityEngine;

public class PlayerMoveState : IPlayerState {
    private float speedPlayer;
    private CharacterController characterPlayer;
    private Animator animationPlayer;
    private Vector3 movementPlayer;


    public Player player;

    public PlayerMoveState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        animationPlayer = player.animator;
        characterPlayer = player.GetComponent<CharacterController>();
        speedPlayer = player.moveSpeed;
    }

    public void Execute() {
        PlayerMovement();
        Gravity();
    }

    public void Exit() {
        animationPlayer.SetFloat("inputX", 0);
        animationPlayer.SetFloat("inputZ", 0);
    }

    private void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movementPlayer = (player.transform.right * horizontal) + (player.transform.forward * vertical);      
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animationPlayer.SetBool("Correr", true);
            movementPlayer = (player.transform.right * horizontal) + (player.transform.forward * vertical) * 2;
        }
        else
        {
            animationPlayer.SetBool("Correr", false);
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
        animationPlayer.SetFloat("inputX", x);
        animationPlayer.SetFloat("inputZ", z);
    }
}
