using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueEndState : IAtaqueState {
    // Holder para fim do ataque
    public AtaqueInstance instance;

    public AtaqueEndState(AtaqueInstance instance) {
        this.instance = instance;
    }

    public void Enter() {
        instance.HandleAtaqueEnd();
    }

    public void Execute() {}

    public void Exit() {}

}
