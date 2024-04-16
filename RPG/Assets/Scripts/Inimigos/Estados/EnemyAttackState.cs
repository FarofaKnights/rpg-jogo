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
        inimigo.animator.SetTrigger("Attack");
        ataqueInstance = inimigo.ataque.Atacar(inimigo);
        ataqueInstance.onHitFinish += ReturnToIdle;
    }

    void ReturnToIdle() {
        inimigo.stateMachine.SetState(inimigo.idleState);
    }

    public void Exit() {
        ataqueInstance.onHitFinish -= ReturnToIdle;
    }

    public void Execute() { }
}
