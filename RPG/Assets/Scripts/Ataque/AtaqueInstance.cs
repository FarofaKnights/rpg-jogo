using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AtaqueInstance {
    public enum Estado { Enter, Antecipacao, Hit, Recovery, End }
    public Estado estadoAtual = Estado.Enter;

    public AtaqueInfo info;
    public IAtacador atacador;
    protected Animator animator;

    public System.Action onEnter, onAntecipacao, onAttack, onRecovery, onEnd;
    public System.Action<Estado> onStateChange;

    bool podeCancelar = false;
    int frameCounter = 0;

    public virtual void OnEnter() { }
    public virtual void OnAntecipacao() { }
    public virtual void OnAttack() { }
    public virtual void OnRecovery() { }
    public virtual void OnEnd() { }


    public AtaqueInstance(AtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        podeCancelar = false;
        
        OnEnter();

        OnStateChange(Estado.Antecipacao);
    }

    public void OnStateChange(Estado estado) {
        frameCounter = 0;
        estadoAtual = estado;

        switch (estado) {
            case Estado.Antecipacao:
                OnAntecipacao();
                onAntecipacao?.Invoke();
                break;
            case Estado.Hit:
                OnAttack();
                onAttack?.Invoke();
                break;
            case Estado.Recovery:
                OnRecovery();
                onRecovery?.Invoke();
                break;
            case Estado.End:
                OnEnd();
                onEnd?.Invoke();
                break;
        }

        onStateChange?.Invoke(estado);
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

    public void PermitirCancelamento() {
        podeCancelar = true;
    }

    public bool Cancelar() {
        if (podeCancelar) {
            OnStateChange(Estado.End);
            return true;
        }

        return false;
    }
}