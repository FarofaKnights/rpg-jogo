using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState : IAtaqueState {
    public AtaqueInfo info;
    public AtaqueInstance instance;

    int frameCounter = 0;

    public RecoveryState(AtaqueInstance instance) {
        this.instance = instance;
        this.info = instance.info;
    }

    public void Enter() {
        frameCounter = 0;
    }

    public void Execute() {
        frameCounter++;
        if (frameCounter >= info.recoveryFrames) {
            instance.stateMachine.SetState(instance.endState);
        }
    }

    public void Exit() {}

}
