using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueUnico: AtaqueComHitbox {
    public AtaqueUnico(AttackBehaviour ataqueBehaviour) : base(ataqueBehaviour) { }


    public override void OnEnterHitbox(GameObject hit) {
        if (GetHit(hit) != null) return;
        HitInfo hitInfo = new HitInfo(hit, 0);
        TriggerHit(hitInfo.hit);
        hits.Add(hitInfo);
    }

    public override void OnLeaveHitbox(GameObject hit) {
        HitInfo hitInfo = GetHit(hit);
        if (hitInfo == null) return;

        hits.Remove(hitInfo);
    }

}