using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueMelee: HitboxAttackBehaviour {
    public GameObject hitbox;
    List<GameObject> hits = new List<GameObject>();

    public AtaqueMelee(MeleeAtaqueInfo info, IAtacador atacador) : base(info, atacador) {
    }

    public override void OnEnd() {
        animator.applyRootMotion = false;
    }


    public override GameObject GetHitbox() {
        GameObject hitbox = new GameObject("AttackHitbox_Melee");
        MeleeAtaqueInfo infoMelee = (MeleeAtaqueInfo)info;

        // Transform
        hitbox.transform.SetParent(atacador.GetInfo().attackHolder.transform);
        hitbox.transform.localPosition = infoMelee.hitboxOffset;
        hitbox.transform.localScale = infoMelee.hitboxSize;
        hitbox.transform.localEulerAngles = infoMelee.hitboxRotation;

        // Trigger
        hitbox.AddComponent<BoxCollider>();
        return hitbox;
    }

    public override void OnHit(GameObject hit) {
        atacador.OnAtaqueHit(hit);
        GameManager.instance.StartCoroutine(SlowdownOnHit());
    }

    IEnumerator SlowdownOnHit() {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.05f);
        if (!GameManager.instance.IsPaused()) Time.timeScale = 1;
        else Time.timeScale = 0;
    }
}
