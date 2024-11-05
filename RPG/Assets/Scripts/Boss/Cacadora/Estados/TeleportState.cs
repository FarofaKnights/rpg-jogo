using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportState : IChainedState {
    public StateMachine<IChainedState> stateMachine;
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

    public TeleportState(StateMachine<IChainedState> stateMachine, NavMeshAgent agent) {
        this.stateMachine = stateMachine;
        this.agent = agent;
    }

    public TeleportState(StateMachine<IChainedState> stateMachine,NavMeshAgent agent, Vector3 target) {
        this.stateMachine = stateMachine;
        this.agent = agent;
        this.target = target;
    }

    public void Enter() {
        agent.enabled = false;
        agent.transform.position = target;
        agent.enabled = true;
        
        agent.ResetPath();
    }

    public void Execute() {
        OnEnd?.Invoke();
        if (nextState != null)
            stateMachine.SetState(nextState);
    }

    public void Exit() { }
}
