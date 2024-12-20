using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHittedState : IEnemyState {
    private Inimigo inimigo;
    float timer, maxTimer;

    Vector3 direction;
    float empurroStep;

    public EnemyHittedState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Hitted";

        inimigo.animator.SetInteger("DamageDir", inimigo.hittedDir);
        inimigo.animator.SetTrigger("Damaged");
        maxTimer = inimigo.tomouDanoStun;
        timer = maxTimer;
        inimigo.SpawnParticle();
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);

        float empurradoDistance = inimigo.empurradoAoSofrerHit;
        direction = Player.instance.transform.forward;

        inimigo.GetComponent<NavMeshAgent>().enabled = false;
        empurroStep = empurradoDistance / (maxTimer/3);
    }

    public void Exit() {
        inimigo.GetComponent<NavMeshAgent>().enabled = true;
    }

    public void Execute() {
        timer -= Time.deltaTime;
        inimigo.controller.Move(direction * empurroStep * Time.deltaTime);

        if (timer < 0) {
            timer = 0;
            inimigo.target = Player.instance.gameObject;
            inimigo.stateMachine.SetState(inimigo.walkState);
        }
    }
}
