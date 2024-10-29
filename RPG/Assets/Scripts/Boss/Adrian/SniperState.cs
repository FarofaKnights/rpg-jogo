using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperState : StateMachineBehaviour {
    public float timeToShoot = 1f;
    float timeToShootCounter = 0f;
    SniperController sniperController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        sniperController = animator.GetComponent<SniperController>();
        timeToShootCounter = 0f;

        sniperController.Mirar();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeToShootCounter += Time.deltaTime;

        if (timeToShootCounter >= timeToShoot) {
            timeToShootCounter = 0f;
            animator.SetTrigger("Atirar");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sniperController.PararDeMirar();
    }
}
