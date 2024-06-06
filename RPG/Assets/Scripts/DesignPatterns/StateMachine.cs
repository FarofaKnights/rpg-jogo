using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : IState {
    T currentState;

    public T GetCurrentState() {
        return currentState;
    }

    public void SetState(T newState) {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Execute() {
        currentState?.Execute();
    }
}
