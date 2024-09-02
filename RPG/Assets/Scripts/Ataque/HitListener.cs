using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe utilizada para relacionar o AtaqueInstance com as variações de ataque
public abstract class HitListener : IAttackEventListener {
    public System.Action<GameObject> onHit;

    public AttackBehaviour ataqueBehavior;
    public AtaqueInfo ataqueInfo;
    public IAtacador atacador;

    public HitListener(AttackBehaviour ataqueBehavior) {
        this.ataqueBehavior = ataqueBehavior;
        this.ataqueInfo = ataqueBehavior.info;
        this.atacador = ataqueBehavior.atacador;
    }

    public void TriggerHit(GameObject hit) {
        ataqueBehavior.OnHit(hit);
    }

    public virtual void OnEnter() { }
    public virtual void OnAntecipacao() { }
    public virtual void OnAttack() { }
    public virtual void OnRecovery() { }
    public virtual void OnEnd() { }
    public virtual void OnUpdate(AtaqueInstance.Estado estado) { }
}
