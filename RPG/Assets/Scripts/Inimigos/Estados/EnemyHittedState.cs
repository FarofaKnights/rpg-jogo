using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHittedState : IEnemyState {
    private Inimigo inimigo;
    float timer, maxTimer;

    public EnemyHittedState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.animator.SetTrigger("Damaged");
        maxTimer = inimigo.tomouDanoStun;
        timer = maxTimer;
        inimigo.SpawnParticle();
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Exit() { }

    public void Execute() {
        timer -= Time.deltaTime;

        if (timer < 0) {
            timer = 0;
            inimigo.stateMachine.SetState(inimigo.walkState);
        }
    }
}
