using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState {
    private Inimigo inimigo;

    public EnemyIdleState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Idle";
        inimigo.ataquesFeitos = 0; // Condicao especial de descanso longo

        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Exit() {
    }

    public void Execute() {
        inimigo.CheckForPlayer();
    }
}
