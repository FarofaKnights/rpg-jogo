using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState {
    private Inimigo inimigo;

    public EnemyIdleState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Exit() {
    }

    public void Execute() {
        // Search in searchRange for a target
        Collider[] colliders = Physics.OverlapSphere(inimigo.transform.position, inimigo.searchRange);
        foreach (Collider collider in colliders) {
            if (collider.CompareTag("Player")) {
                inimigo.target = collider.gameObject;
                inimigo.stateMachine.SetState(inimigo.walkState);
                return;
            }
        }
    }
}
