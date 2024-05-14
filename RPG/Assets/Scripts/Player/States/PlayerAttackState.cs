using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState {
    public Player player;
    int comboCount = 0;
    int comboIndex = 0;
    bool comboFull = false;

    public PlayerAttackState(Player player) {
        this.player = player;
    }

    public void Enter() {
        if (player.arma == null) {
            player.stateMachine.SetState(player.moveState);
            return;
        }

        comboCount = 0;
        comboIndex = 0;
        comboFull = false;

        //player.arma.onAttackEnd += onAttackEnd;
        Atacar();
    }

    public void Execute() {
        if (Input.GetMouseButtonDown(0)) {
            if (comboFull) return;
            comboCount++;
            if (player.arma.ataques.Length <= comboCount + 1) comboFull = true;
        }
    }

    public void Exit() {
        //player.arma.onAttackEnd -= onAttackEnd;
        if (player.arma != null)
            player.arma.Resetar();
    }

    void Atacar() {
        player.arma.Atacar();
    }

    public void onAttackEnd() {
        if (player.arma != null && comboCount > 0 && comboIndex < comboCount) {
            comboIndex++;
            Atacar();
        } else {
            player.stateMachine.SetState(player.moveState);
        }
    }
}
