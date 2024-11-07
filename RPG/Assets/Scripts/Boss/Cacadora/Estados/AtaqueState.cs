using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueState : IChainedState {
    public IAtacador atacador;
    public AtaqueInfo ataque;
    public AtaqueInstance ataqueInstance;

    
    public AtaqueState(StateMachine<IChainedState> stateMachine, IAtacador atacador, AtaqueInfo ataque) {
        this.stateMachine = stateMachine;
        this.atacador = atacador;
        this.ataque = ataque;
    }

    public override void Enter() {
        ataqueInstance = ataque.Atacar(atacador);
        ataqueInstance.onEnd += Next;
    }

    public override void Execute() {
        if (ataqueInstance != null) {
            ataqueInstance.Update();
        }
    }

    public override void Exit() {
        if (ataqueInstance != null)
            ataqueInstance.onEnd -= Next;
        
        ataqueInstance = null;
    }
}
