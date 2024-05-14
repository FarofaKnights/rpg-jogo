using UnityEngine;

public class PlayerMoveState : IPlayerState {
    PlayerMoviment scriptMovimento;


    public Player player;

    public PlayerMoveState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        scriptMovimento = player.GetComponent<PlayerMoviment>();
        scriptMovimento.podeAndar = true;
    }

    public void Execute() {
    }

    public void Exit() {
        scriptMovimento.podeAndar = false;
    }

}
