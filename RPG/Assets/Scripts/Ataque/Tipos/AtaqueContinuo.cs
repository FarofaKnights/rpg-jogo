using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueContinuo: AtaqueComHitbox {
    public float frequenciaDeDano;

    public AtaqueContinuo(AttackBehaviour ataqueBehaviour) : base(ataqueBehaviour) {
        frequenciaDeDano = ataqueInfo.frequenciaDeDano;
    }


    public override void OnEnterHitbox(GameObject hit) {
        if (GetHit(hit) != null) return;
        HitInfo hitInfo = new HitInfo(hit, frequenciaDeDano);
        OnHitTime(hitInfo);
        hits.Add(hitInfo);
    }

    public override void OnLeaveHitbox(GameObject hit) {
        HitInfo hitInfo = GetHit(hit);
        if (hitInfo == null) return;

        hits.Remove(hitInfo);
    }

    public override void OnUpdate(AtaqueInstance.Estado estado) {
        UpdateHitTime(Time.deltaTime);
    }

    protected void UpdateHitTime(float deltaTime) {
        
        for (int i = 0; i < hits.Count; i++) {
            HitInfo hitInfo = hits[i];
            hitInfo.time -= deltaTime;

            if (hitInfo.time <= 0) {
                OnHitTime(hitInfo);
            }
        }
    }

    public void OnHitTime(HitInfo hitInfo) {
        if (hitInfo == null) return;
        TriggerHit(hitInfo.hit);
        hitInfo.time = frequenciaDeDano;
    }
}