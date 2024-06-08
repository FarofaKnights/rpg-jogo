using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : IPlayerState {
    public PlayerMovimentacao playerMovimentacao;
    public Player player;

    public PlayerMoveState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        playerMovimentacao = player.GetComponent<PlayerMovimentacao>();
        playerMovimentacao.isWalkState = true;
    }

    public void Execute() { }

    public void Exit() {
        playerMovimentacao.isWalkState = false;
    }
}