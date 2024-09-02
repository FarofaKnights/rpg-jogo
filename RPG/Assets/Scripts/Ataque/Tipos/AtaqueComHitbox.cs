using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Essa é a classe abstrata a se usar se o dano for calculado por uma hitbox
// Isso engloba dano único de colisão, contínuo por área e de animação por trigger
public abstract class AtaqueComHitbox : HitListener {    
    public GameObject hitbox;
    public HitboxAttackBehaviour hitboxBehavior;

    public class HitInfo {
        public GameObject hit;
        public float time;

        public HitInfo() {
            hit = null;
            time = 0;
        }

        public HitInfo(GameObject hit, float time) {
            this.hit = hit;
            this.time = time;
        }
    }
    public List<HitInfo> hits = new List<HitInfo>();

    public AtaqueComHitbox(AttackBehaviour ataqueBehaviour): base(ataqueBehaviour) {
        hitboxBehavior = (HitboxAttackBehaviour) ataqueBehaviour;
        CreateHitbox();
    }

    public abstract void OnEnterHitbox(GameObject hit);
    public abstract void OnLeaveHitbox(GameObject hit);

    protected void CreateHitbox() {
        hitbox = hitboxBehavior.GetHitbox();

        // Trigger
        hitbox.GetComponent<Collider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>().onTriggerEnter += OnEnterHitbox;
        hitbox.GetComponent<OnTrigger>().onTriggerExit += OnLeaveHitbox;

        DesativarHitbox();
    }

    public override void OnAttack() {
        AtivarHitbox();
    }

    public override void OnRecovery() {
        if (hitbox != null) {
            hitbox.SetActive(false);
            Object.Destroy(hitbox);

            hits.Clear();
        }
    }

    public override void OnEnd() {
        if (hitbox != null) {
            hitbox.SetActive(false);
            Object.Destroy(hitbox);

            hits.Clear();
        }
    }

    public void AtivarHitbox() {
        if (hitbox != null)
            hitbox.SetActive(true);
    }

    public void DesativarHitbox() {
        if (hitbox != null)
            hitbox.SetActive(false);
    }

    public HitInfo GetHit(GameObject hit) {
        foreach (HitInfo hitInfo in hits) {
            if (hitInfo.hit == hit) return hitInfo;
        }

        return null;
    }
}