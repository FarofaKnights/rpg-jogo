using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
    IState currentState;

    public IState GetCurrentState() {
        return currentState;
    }

    public void SetState(IState newState) {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Execute() {
        currentState?.Execute();
    }
}
