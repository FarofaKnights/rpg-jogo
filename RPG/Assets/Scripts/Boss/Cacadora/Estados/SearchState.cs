using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : IChainedState {
    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;
    public float range;

    public SearchState(StateMachine<IChainedState> stateMachine, Animator animator, NavMeshAgent agent) {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.agent = agent;
        this.range = agent.stoppingDistance;
    }

    public SearchState(StateMachine<IChainedState> stateMachine, Animator animator, NavMeshAgent agent, Transform target, float range) {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.agent = agent;
        this.target = target;
        this.range = range;
    }

    public override void Enter() {
        agent.SetDestination(target.position);
    }

    public override void Execute() {
        agent.SetDestination(target.position);

        Vector3 velocity = agent.velocity.normalized;
        float speed = agent.velocity.magnitude;

        float vertical = Vector3.Dot(velocity, agent.transform.forward);
        float horizontal = Vector3.Dot(velocity, agent.transform.right);

        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Horizontal", horizontal);

        agent.transform.LookAt(target);

        if (agent.remainingDistance <= range) {
            Next();
        }
    }

    public override void Exit() {
        animator.SetFloat("Vertical", 0);
        animator.SetFloat("Horizontal", 0);

        agent.ResetPath();
    }
}
