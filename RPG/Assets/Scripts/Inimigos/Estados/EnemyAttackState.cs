using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IEnemyState {
    private Inimigo inimigo;
    AtaqueInstance ataqueInstance;

    public EnemyAttackState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        GameManager.instance.StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        yield return new WaitForSeconds(0.25f);
        StartAttack();
    }

    void StartAttack() {
        if (inimigo == null) return;

        ataqueInstance = inimigo.ataque.Atacar(inimigo);
        ataqueInstance.onEnd += ReturnToIdle;
    }

    void ReturnToIdle() {
        inimigo.stateMachine.SetState(inimigo.idleState);
    }

    public void Exit() {
        if (ataqueInstance != null)
            ataqueInstance.onEnd -= ReturnToIdle;
    }

    public void Execute() {
        if (ataqueInstance != null) {
            ataqueInstance.Update();
        }
    }
}
