using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHittedState : IPlayerState {
    public Player player;

    // Referências
    CharacterController controller;
    Animator animator;
    PossuiVida vidaPlayer;

    float timer = 0;


    public PlayerHittedState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        animator = player.animator;
        controller = player.GetComponent<CharacterController>();
        vidaPlayer = player.GetComponent<PossuiVida>();

        animator.SetTrigger("Hitted");

        timer = 0;
    }

    public void Update() { }

    public void Execute() {
        timer += Time.deltaTime;

        if (timer >= player.stunTime) {
            player.stateMachine.SetState(player.moveState);
        }
    }

    public void Exit() { }

    void SetInvulnerable(bool val) {
        vidaPlayer.SetInvulneravel(val);
    }
}