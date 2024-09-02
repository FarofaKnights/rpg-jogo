using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : IAttackEventListener {
    public AtaqueInfo info;
    public IAtacador atacador;
    public Animator animator;


    public AttackBehaviour(AtaqueInfo ataqueInfo, IAtacador atacador) {
        this.info = ataqueInfo;
        this.atacador = atacador;
        this.animator = atacador.GetAnimator();
    }

    public abstract void OnHit(GameObject hit);

    public virtual void OnEnter() { }
    public virtual void OnAntecipacao() { }
    public virtual void OnAttack() { }
    public virtual void OnRecovery() { }
    public virtual void OnEnd() { }
    public virtual void OnUpdate(AtaqueInstance.Estado estado) { }
}
