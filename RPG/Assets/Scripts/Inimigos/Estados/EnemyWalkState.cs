using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkState : IEnemyState {
    private Inimigo inimigo;
    private NavMeshAgent navMeshAgent;

    public EnemyWalkState(Inimigo inimigo) {
        this.inimigo = inimigo;
        navMeshAgent = inimigo.GetComponent<NavMeshAgent>();
    }

    public void Enter() {
        GameObject target = inimigo.target;

        if (target == null) {
            inimigo.stateMachine.SetState(inimigo.idleState);
            return;
        }

        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(target.transform.position);
    }

    public void Exit() {
        navMeshAgent.enabled = false;
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Execute() {
        navMeshAgent.SetDestination(inimigo.target.transform.position);

        Vector3 velocity = navMeshAgent.velocity.normalized;
        inimigo.animator.SetFloat("Vertical", velocity.z);
        inimigo.animator.SetFloat("Horizontal", velocity.x);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            inimigo.stateMachine.SetState(inimigo.attackState);
        }
    }
}
