using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHittedState : IEnemyState {
    private Inimigo inimigo;
    float timer, maxTimer = 1f;

    public EnemyHittedState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.animator.SetTrigger("Damaged");
        timer = maxTimer;

        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
        
        Debug.Log("React to damage");
    }

    public void Exit() { }

    public void Execute() {
        timer -= Time.deltaTime;

        if (timer < 0) {
            timer = 0;
            inimigo.stateMachine.SetState(inimigo.idleState);
        }
    }
}
