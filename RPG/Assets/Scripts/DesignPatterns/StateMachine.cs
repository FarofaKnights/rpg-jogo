using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : IState {
    public System.Action<T> OnStateChange;
    T currentState;

    public T GetCurrentState() {
        return currentState;
    }

    public void SetState(T newState) {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
        
        OnStateChange?.Invoke(currentState);
    }

    public void Execute() {
        currentState?.Execute();
    }
}
