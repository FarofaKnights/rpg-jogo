using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IAtaqueState {
    public AtaqueInfo info;
    public AtaqueInstance instance;

    int frameCounter = 0;

    public HitState(AtaqueInstance instance) {
        this.instance = instance;
        this.info = instance.info;
    }

    public void Enter() {
        frameCounter = 0;

        instance.AtivarHitbox();
    }

    public void Execute() {
        frameCounter++;
        if (frameCounter >= info.hitFrames) {
            instance.stateMachine.SetState(instance.recoveryState);
        }
        
    }

    public void Exit() {
        instance.DesativarHitbox();
    }

}
