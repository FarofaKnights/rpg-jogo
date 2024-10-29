using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsperarState : StateMachineBehaviour
{
    public float tempoEspera = 1f;
    float tempoEsperaAtual = 0f;

    public enum Tipo { TRIGGER, BOOL }

    public string trigger;
    public Tipo tipo = Tipo.TRIGGER;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        tempoEsperaAtual = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        tempoEsperaAtual += Time.deltaTime;

        if (tempoEsperaAtual >= tempoEspera) {
            tempoEsperaAtual = 0f;
            Disparar(animator);
        }
    }

    public void Disparar(Animator animator) {
        if (tipo == Tipo.TRIGGER) animator.SetTrigger(trigger);
        else animator.SetBool(trigger, true);
    }
}
