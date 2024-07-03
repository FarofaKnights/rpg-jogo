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
    int totalFrameCounter = 0;

    public virtual void OnEnter() { }
    public virtual void OnAntecipacao() { }
    public virtual void OnAttack() { }
    public virtual void OnRecovery() { }
    public virtual void OnEnd() { }

    public virtual void OnUpdate(Estado estado) { }


    public AtaqueInstance(AtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        podeCancelar = false;

        if (info.animatorOverride != null) {
            animator = atacador.GetAnimator();
            animator.runtimeAnimatorController = info.animatorOverride;
            animator.applyRootMotion = false;

            if (atacador.GetTriggerMode() == TriggerMode.Trigger) {
                animator.SetTrigger(atacador.AttackTriggerName());
            } else if (atacador.GetTriggerMode() == TriggerMode.Bool) {
                animator.SetBool(atacador.AttackTriggerName(), true);
            }
        }

        AtaqueAnimationEvents ataqueAnimationEvents = atacador.GetAnimator().gameObject.GetComponent<AtaqueAnimationEvents>();
        if (ataqueAnimationEvents != null && !info.usarTiming) {
            ataqueAnimationEvents.ataqueInstance = this;
        }
       
        totalFrameCounter = 0;
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

                if (info.animatorOverride != null && atacador.GetTriggerMode() == TriggerMode.Bool) {
                    animator.SetBool(atacador.AttackTriggerName(), false);
                }

                break;
        }

        onStateChange?.Invoke(estado);
    }

    public void Update() {
        frameCounter++;
        totalFrameCounter++;

        if (info.moveDuranteAtaque && totalFrameCounter >= info.moveStartFrame && totalFrameCounter <= info.moveEndFrame) {
            MoveWithAttack();
        }

        if (!info.usarTiming) {
            return;
        }

        if (estadoAtual == Estado.Antecipacao) {
            if (frameCounter >= info.antecipacaoFrames) OnStateChange(Estado.Hit);
        } else if (estadoAtual == Estado.Hit) {
            if (frameCounter >= info.hitFrames) OnStateChange(Estado.Recovery);
        } else if (estadoAtual == Estado.Recovery) {
            if (frameCounter >= info.recoveryFrames) OnStateChange(Estado.End);
        }

        if (!podeCancelar && info.cancelFrames >= 0 && totalFrameCounter >= info.cancelFrames) {
            PermitirCancelamento();
        }

        OnUpdate(estadoAtual);
    }

    public void MoveWithAttack() {
        if (info.moveDuranteAtaque) {
            float totalTime = info.moveEndFrame - info.moveStartFrame;
            float speed = info.moveDistance / totalTime;
            float progress = totalFrameCounter - info.moveStartFrame;
            float progressPercent = progress / totalTime;

            atacador.MoveWithAttack(speed, progressPercent);
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