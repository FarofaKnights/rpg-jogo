using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AdrianSearchState : StateMachineBehaviour
{
    public float longeRadius = 10f;
    public float medioRadius = 5f;
    public float pertoRadius = 2f;
    public float bracoCooldown = 2f;
    float bracoCooldownTimer = 0f;

    Transform player;
    NavMeshAgent agent;
    SniperController sniperController;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = Player.instance.transform;
        sniperController = animator.GetComponent<SniperController>();
        agent = sniperController.agent;

        bracoCooldownTimer = 0f;

        agent.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance >= longeRadius) {
            bracoCooldownTimer = 0;
            agent.enabled = false;
            animator.SetTrigger("Atirar");
        } else if (distance <= pertoRadius) {
            agent.enabled = false;
            animator.SetTrigger("Coronhada");
        } else {
            agent.SetDestination(player.position);
            agent.enabled = true;

            if (bracoCooldownTimer < bracoCooldown) {
                bracoCooldownTimer += Time.deltaTime;
            }

            if (distance <= medioRadius && bracoCooldownTimer >= bracoCooldown) {
                agent.enabled = false;
                animator.SetTrigger("Braco");
            }
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
