using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueAnimacao: AtaqueComHitbox {
    public AtaqueAnimacao(AttackBehaviour ataqueBehaviour) : base(ataqueBehaviour) { }

    // Chama o hit pela animação
    public void TriggerAHit() {
        foreach (HitInfo hit in hits) {
            TriggerHit(hit.hit);
        }
    }

    public override void OnEnterHitbox(GameObject hit) {
        if (GetHit(hit) != null) return;
        HitInfo hitInfo = new HitInfo(hit, 0);
        hits.Add(hitInfo);
    }

    public override void OnLeaveHitbox(GameObject hit) {
        HitInfo hitInfo = GetHit(hit);
        if (hitInfo == null) return;

        hits.Remove(hitInfo);
    }

}