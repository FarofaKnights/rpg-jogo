using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AdrianSearchState : StateMachineBehaviour
{
    public float cooldownStart = 2f;
    float cooldownStartTimer = 0f;
    bool comecou = false;

    public float longeRadius = 10f;
    public float medioRadius = 5f;
    public float pertoRadius = 2f;
    public float bracoCooldown = 2f;
    float bracoCooldownTimer = 0f;

    Transform player;
    NavMeshAgent agent;
    SniperController sniperController;
    CharacterController characterController;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = Player.instance.transform;
        sniperController = animator.GetComponent<SniperController>();
        agent = sniperController.agent;


        characterController = sniperController.controller;
        characterController.enabled = true;

        bracoCooldownTimer = 0f;
        cooldownStartTimer = 0f;
        comecou = false;
        
        agent.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!comecou) {
            if (cooldownStartTimer < cooldownStart) {
                cooldownStartTimer += Time.deltaTime;
                return;
            } else {
                comecou = true;
            }
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance >= longeRadius) {
            bracoCooldownTimer = 0;
            agent.enabled = false;
            sniperController.Mirar();
            animator.SetTrigger("Atirar");
        } else if (distance <= pertoRadius) {
            agent.enabled = false;
            animator.SetTrigger("Coronhada");
        } else {
            agent.enabled = true;
            agent.SetDestination(player.position);

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
