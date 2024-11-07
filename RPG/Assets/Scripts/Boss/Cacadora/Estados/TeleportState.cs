using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportState : IChainedState {
    public NavMeshAgent agent;
    public Vector3 target;

    public TeleportState(StateMachine<IChainedState> stateMachine, NavMeshAgent agent) {
        this.stateMachine = stateMachine;
        this.agent = agent;
    }

    public TeleportState(StateMachine<IChainedState> stateMachine,NavMeshAgent agent, Vector3 target) {
        this.stateMachine = stateMachine;
        this.agent = agent;
        this.target = target;
    }

    public override void Enter() {
        agent.enabled = false;
        agent.transform.position = target;
        agent.enabled = true;
        
        agent.ResetPath();
    }

    public override void Execute() {
        Next();
    }

    public override void Exit() { }
}
