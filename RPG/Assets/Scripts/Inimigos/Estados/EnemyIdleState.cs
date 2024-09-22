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
        Collider[] colliders = Physics.OverlapSphere(inimigo.transform.position, inimigo.rangeProcurando);
        foreach (Collider collider in colliders) {
            float yDist = Mathf.Abs(collider.transform.position.y - inimigo.transform.position.y);
            if (yDist > inimigo.maxYProcurando) continue;

            if (collider.CompareTag("Player")) {
                inimigo.target = collider.gameObject;
                inimigo.stateMachine.SetState(inimigo.walkState);
                return;
            }
        }
    }
}
