using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntecipacaoState : IAtaqueState {
    public AtaqueInfo info;
    public AtaqueInstance instance;

    int frameCounter = 0;

    public AntecipacaoState(AtaqueInstance instance) {
        this.instance = instance;
        this.info = instance.info;
    }

    public void Enter() {
        frameCounter = 0;
    }

    public void Execute() {
        frameCounter++;
        if (frameCounter >= info.antecipacaoFrames) {
            instance.stateMachine.SetState(instance.hitState);
        }
    }

    public void Exit() {}
}
