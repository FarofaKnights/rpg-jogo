using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AtaqueInstance {
    public enum Estado { Enter, Antecipacao, Hit, Recovery, End }
    public Estado estadoAtual = Estado.Enter;

    public MeleeAtaqueInfo info;
    public IAtacador atacador;
    protected Animator animator;

    public System.Action onEnd;

    int frameCounter = 0;

    public virtual void OnEnter() { }
    public virtual void OnAntecipacao() { }
    public virtual void OnAttack() { }
    public virtual void OnRecovery() { }
    public virtual void OnEnd() { }


    public AtaqueInstance(MeleeAtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        
        OnEnter();

        OnStateChange(Estado.Antecipacao);
    }

    public void OnStateChange(Estado estado) {
        frameCounter = 0;
        estadoAtual = estado;

        switch (estado) {
            case Estado.Antecipacao:
                OnAntecipacao();
                break;
            case Estado.Hit:
                OnAttack();
                break;
            case Estado.Recovery:
                OnRecovery();
                break;
            case Estado.End:
                OnEnd();
                break;
        }
    }

    public void Update() {
        frameCounter++;

        if (estadoAtual == Estado.Antecipacao) {
            if (frameCounter >= info.antecipacaoFrames) OnStateChange(Estado.Hit);
        } else if (estadoAtual == Estado.Hit) {
            if (frameCounter >= info.hitFrames) OnStateChange(Estado.Recovery);
        } else if (estadoAtual == Estado.Recovery) {
            if (frameCounter >= info.recoveryFrames) OnStateChange(Estado.End);
        }
    }
}