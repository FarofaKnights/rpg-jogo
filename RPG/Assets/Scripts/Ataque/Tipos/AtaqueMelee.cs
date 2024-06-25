using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueMelee: AtaqueInstance {
    public GameObject hitbox;
    List<GameObject> hits = new List<GameObject>();

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
    }


    protected void CreateHitbox() {
        hitbox = new GameObject("AttackHitbox");
        MeleeAtaqueInfo infoMelee = (MeleeAtaqueInfo)info;

        // Transform
        hitbox.transform.SetParent(atacador.GetAttackHolder().transform);
        hitbox.transform.localPosition = infoMelee.hitboxOffset;
        hitbox.transform.localScale = infoMelee.hitboxSize;
        hitbox.transform.localEulerAngles = infoMelee.hitboxRotation;

        // Trigger
        hitbox.AddComponent<BoxCollider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>().onTriggerEnter += OnHitATarget;

        hitbox.SetActive(false);
    }

    void OnHitATarget(GameObject hit) {
        if (hits.Contains(hit)) return;
        hits.Add(hit);

        atacador.OnAtaqueHit(hit);
        GameManager.instance.StartCoroutine(SlowdownOnHit());
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
