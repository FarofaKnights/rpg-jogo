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
        // inimigo.attackSound.Play();
        GameManager.instance.StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        yield return new WaitForSeconds(0.25f);
        StartAttack();
    }

    void StartAttack() {
        if (inimigo == null) return;

        ataqueInstance = inimigo.ataque.Atacar(inimigo);
        ataqueInstance.onEnd += LeaveState;

        ataqueInstance.onAttack += () => {
            inimigo.attackSound.Play();
        };

        ataqueInstance.onRecovery += () => {
            inimigo.attackSound.Stop();
        };
    }

    void LeaveState() {
        inimigo.stateMachine.SetState(inimigo.walkState);
    }

    public void Exit() {
        // inimigo.attackSound.Stop();
        if (ataqueInstance != null)
            ataqueInstance.onEnd -= LeaveState;
    }

    public void Execute() {
        if (ataqueInstance != null) {
            ataqueInstance.Update();
        }
    }
}
