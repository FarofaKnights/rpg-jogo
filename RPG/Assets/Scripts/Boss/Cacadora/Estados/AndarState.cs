using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AndarState : IChainedState {
    public StateMachine<IChainedState> stateMachine;
    public Animator animator;
    public NavMeshAgent agent;
    public Vector3 target;

    protected IChainedState _nextState;
    public IChainedState nextState {
        get { return _nextState; }
        set { _nextState = value; }
    }

    protected System.Action _OnEnd;
    public System.Action OnEnd {
        get { return _OnEnd; }
        set { _OnEnd = value; }
    }

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

    public void Enter() {
        agent.SetDestination(target);
    }

    public void Execute() {
        Vector3 velocity = agent.velocity.normalized;
        float speed = agent.velocity.magnitude;

        float vertical = Vector3.Dot(velocity, agent.transform.forward);
        float horizontal = Vector3.Dot(velocity, agent.transform.right);

        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Horizontal", horizontal);

        if (agent.remainingDistance <= agent.stoppingDistance) {
            OnEnd?.Invoke();
            if (nextState != null)
                stateMachine.SetState(nextState);
        }
    }

    public void Exit() {
        animator.SetFloat("Vertical", 0);
        animator.SetFloat("Horizontal", 0);

        agent.ResetPath();
    }
}
