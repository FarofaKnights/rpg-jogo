using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueMelee: AtaqueInstance {
    public GameObject hitbox;

    public AtaqueMelee(MeleeAtaqueInfo info, IAtacador atacador) : base(info, atacador) {
        CreateHitbox();

        animator = atacador.GetAnimator();
        animator.runtimeAnimatorController = info.animatorOverride;
        animator.applyRootMotion = false;
        animator.SetTrigger(atacador.AttackTriggerName());
    }

    public override void OnAttack() {
        AtivarHitbox();
    }

    public override void OnRecovery() {
        DesativarHitbox();
    }

    public override void OnEnd() {
        if (hitbox != null) {
            hitbox.SetActive(false);
            Object.Destroy(hitbox);
        }
    
        animator.applyRootMotion = false;
        if (onEnd != null) onEnd();
    }


    protected void CreateHitbox() {
        hitbox = new GameObject("AttackHitbox");

        // Transform
        hitbox.transform.SetParent(atacador.GetAttackHitboxHolder().transform);
        hitbox.transform.localPosition = info.hitboxOffset;
        hitbox.transform.localScale = info.hitboxSize;
        hitbox.transform.localEulerAngles = info.hitboxRotation;

        // Trigger
        hitbox.AddComponent<BoxCollider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>().onTriggerEnter += (GameObject hit) => {
            atacador.OnAtaqueHit(hit);
            GameManager.instance.StartCoroutine(SlowdownOnHit());
        };

        hitbox.SetActive(false);
    }

    IEnumerator SlowdownOnHit() {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.05f);
        if (!GameManager.instance.IsPaused()) Time.timeScale = 1;
        else Time.timeScale = 0;
    }

    public void AtivarHitbox() {
        hitbox.SetActive(true);
    }

    public void DesativarHitbox() {
        hitbox.SetActive(false);
    }
}
