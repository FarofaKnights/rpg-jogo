using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperState : StateMachineBehaviour {
    public float timeToShoot = 1f;
    float timeToShootCounter = 0f;
    bool mirando = false;
    SniperController sniperController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        sniperController = animator.GetComponent<SniperController>();
        timeToShootCounter = 0f;
        mirando = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Atirando")) {
            if (!mirando) {
                sniperController.Mirar();
                mirando = true;
            }

            timeToShootCounter += Time.deltaTime;

            if (timeToShootCounter >= timeToShoot) {
                timeToShootCounter = 0f;
                animator.SetTrigger("Atirar");
                mirando = false;
            }
        } else if (mirando) {
            sniperController.PararDeMirar();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }
}
