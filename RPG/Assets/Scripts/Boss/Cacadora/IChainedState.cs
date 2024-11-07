using UnityEngine;

public abstract class IChainedState: IState {
    public StateMachine<IChainedState> stateMachine;
    public IChainedState nextState;
    public System.Action OnEnd;
    
    public void Next() {
        OnEnd?.Invoke();
        if (nextState != null)
            stateMachine.SetState(nextState);
    }
    
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}