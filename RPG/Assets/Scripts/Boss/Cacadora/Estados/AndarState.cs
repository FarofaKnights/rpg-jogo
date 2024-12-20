using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AndarState : IChainedState {
    public Animator animator;
    public NavMeshAgent agent;
    public Vector3 target;

    public AndarState(StateMachine<IChainedState> stateMachine, Animator animator, NavMeshAgent agent) {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.agent = agent;
    }

    public AndarState(StateMachine<IChainedState> stateMachine, Animator animator, NavMeshAgent agent, Vector3 target) {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.agent = agent;
        this.target = target;
    }

    public override void Enter() {
        agent.SetDestination(target);
    }

    public override void Execute() {
        Vector3 velocity = agent.velocity.normalized;
        float speed = agent.velocity.magnitude;

        float vertical = Vector3.Dot(velocity, agent.transform.forward);
        float horizontal = Vector3.Dot(velocity, agent.transform.right);

        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Horizontal", horizontal);

        if (agent.remainingDistance <= agent.stoppingDistance) {
            Next();
        }
    }

    public override void Exit() {
        animator.SetFloat("Vertical", 0);
        animator.SetFloat("Horizontal", 0);

        agent.ResetPath();
    }
}
