using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackEventListener {
    void OnEnter() { }
    void OnAntecipacao() { }
    void OnAttack() { }
    void OnRecovery() { }
    void OnEnd() { }

    void OnUpdate(AtaqueInstance.Estado estado) { }
}


public class AtaqueInstance  {
    public enum Estado { Enter, Antecipacao, Hit, Recovery, End }
    public Estado estadoAtual = Estado.Enter;

    public HitListener hitListener;
    public AttackBehaviour attackBehaviour;

    public AtaqueInfo info;
    public IAtacador atacador;
    public AtacadorInfo atacadorInfo;
    protected Animator animator;

    public System.Action onEnter, onAntecipacao, onAttack, onRecovery, onEnd;
    public System.Action<Estado> onStateChange;

    bool podeCancelar = false;
    bool isMovingOnTrigger = false;
    int frameCounter = 0;
    int totalFrameCounter = 0;

    // Propriedade relacionada ao descanso longo (caso houver)
    float recoveryMultiplier = 1;

    public virtual void OnEnter() {
        hitListener.OnEnter();
        attackBehaviour.OnEnter();
    }

    public virtual void OnAntecipacao() {
        hitListener.OnAntecipacao();
        attackBehaviour.OnAntecipacao();
    }

    public virtual void OnAttack() {
        hitListener.OnAttack();
        attackBehaviour.OnAttack();
    }

    public virtual void OnRecovery() {
        hitListener.OnRecovery();
        attackBehaviour.OnRecovery();
    }

    public virtual void OnEnd() {
        hitListener.OnEnd();
        attackBehaviour.OnEnd();
    }

    public virtual void OnUpdate(Estado estado) {
        hitListener.OnUpdate(estado);
        attackBehaviour.OnUpdate(estado);
    }


    public AtaqueInstance(AtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        podeCancelar = false;

        atacadorInfo = atacador.GetInfo();
        animator = atacadorInfo.animator;

        if (info.animatorOverride != null) {
            animator.runtimeAnimatorController = info.animatorOverride;
        }

        animator.applyRootMotion = false;

        if (atacadorInfo.triggerMode == TriggerMode.Trigger) {
            animator.SetTrigger(atacadorInfo.attackTriggerName);
        } else if (atacadorInfo.triggerMode == TriggerMode.Bool) {
            animator.SetBool(atacadorInfo.attackTriggerName, true);
        }

        AtaqueAnimationEvents ataqueAnimationEvents = animator.gameObject.GetComponent<AtaqueAnimationEvents>();
        if (ataqueAnimationEvents != null) {
            ataqueAnimationEvents.ataqueInstance = this;
        }

        this.attackBehaviour = info.GetBehaviour(atacador);
        hitListener = info.GetHitListener(this.attackBehaviour);
       
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

                if (info.animatorOverride != null && atacadorInfo.triggerMode == TriggerMode.Bool) {
                    animator.SetBool(atacadorInfo.attackTriggerName, false);
                }

                break;
        }

        onStateChange?.Invoke(estado);
    }

    public void Update() {
        frameCounter++;
        totalFrameCounter++;

        if (info.moveDuranteAtaque) {
            if (info.moveWithTiming && totalFrameCounter >= info.moveStartFrame && totalFrameCounter <= info.moveEndFrame)
                MoveWithAttack();
            else if (info.moveWithTrigger && isMovingOnTrigger){
                atacador.MoveWithAttack(info.moveOnTriggerSpeed, Time.fixedDeltaTime);
            }

        }

        if (info.usarTiming) {
            if (estadoAtual == Estado.Antecipacao) {
                if (frameCounter >= info.antecipacaoFrames) OnStateChange(Estado.Hit);
            } else if (estadoAtual == Estado.Hit) {
                if (frameCounter >= info.hitFrames) OnStateChange(Estado.Recovery);
            } else if (estadoAtual == Estado.Recovery) {
                if (frameCounter >= info.recoveryFrames * recoveryMultiplier) OnStateChange(Estado.End);
            }

            if (!podeCancelar && info.cancelFrames >= 0 && totalFrameCounter >= info.cancelFrames) {
                PermitirCancelamento();
            }
        }

        OnUpdate(estadoAtual);
    }

    public void MoveWithAttack() {
        if (!info.moveDuranteAtaque) return;

        if (info.moveWithTiming) {
            float totalTime = info.moveEndFrame - info.moveStartFrame;
            float speed = info.moveDistance / totalTime;
            float progress = totalFrameCounter - info.moveStartFrame;
            float progressPercent = progress / totalTime;

            atacador.MoveWithAttack(speed, progressPercent);
        } else if (info.moveWithTrigger) {
            isMovingOnTrigger = true;
        }
    }

    public void EndMoveWithAttack() {
        if (isMovingOnTrigger) {
            isMovingOnTrigger = false;
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

    // Força o fim do ataque, normalmente usado quando o ataque é interrompido por um evento externo
    public void Acabou() {
        OnStateChange(Estado.End);
    }

    public bool PodeCancelar() {
        return podeCancelar;
    }

    public void TriggerAHit() {
        if (typeof(AtaqueAnimacao).IsInstanceOfType(hitListener)) {
            ((AtaqueAnimacao)hitListener).TriggerAHit();
        }
    }

    public void RecoveryLongo(float recoveryMultiplier = 2f) {
        this.recoveryMultiplier = recoveryMultiplier;
    }
}