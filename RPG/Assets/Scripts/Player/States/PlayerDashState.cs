using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : IPlayerState {
    public Player player;

    // Atributos
    Vector3 velocity;
    bool isGrounded;
    float rollTimer;
    Vector3 rollDirection;


    // Referências
    StateMachine<IPlayerState> stateMachine;
    PlayerMovementInfo info;
    CharacterController controller;
    Animator animator;
    PossuiVida vidaPlayer;
    Transform mainCameraTransform;


    public PlayerDashState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        animator = player.animator;
        info = player.informacoesMovimentacao;
        stateMachine = player.stateMachine;

        controller = player.GetComponent<CharacterController>();
        vidaPlayer = player.GetComponent<PossuiVida>();
        mainCameraTransform = Camera.main.transform;
        rollTimer = info.rollDuration;

        float x = player.velocity.x == 0 ? 0 : Mathf.Sign(player.velocity.x);
        float z = player.velocity.z == 0 ? 0 : Mathf.Sign(player.velocity.z);
        player.velocity = Vector3.zero;

        rollDirection =  player.transform.forward * z + player.transform.right * x;

        animator.SetFloat("inputX", x);
        animator.SetFloat("inputZ", z);
        animator.SetBool("Rolamento", true);

        SetInvulnerable(true);
    }

    public void Update() { }

    public void Execute() {
        if (rollTimer > 0) {
            controller.Move(rollDirection * info.rollSpeed * Time.deltaTime);
            rollTimer -= Time.deltaTime;
        } else {
            stateMachine.SetState(player.moveState);
        }
    }

    public void Exit() {
        animator.SetBool("Rolamento", false);
        SetInvulnerable(false);
    }

    void SetInvulnerable(bool val) {
        vidaPlayer.SetInvulneravel(val);
    }
}