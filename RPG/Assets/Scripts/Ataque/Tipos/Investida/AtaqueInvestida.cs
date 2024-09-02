using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtaqueInvestida : HitboxAttackBehaviour {
    public float velocidade;
    public int dano;

    public AtaqueInvestida(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        InvestidaAtaqueInfo info = (InvestidaAtaqueInfo)ataqueInfo;
        velocidade = info.velocidade;
        dano = info.dano;
    }

    public override GameObject GetHitbox() {
        GameObject hitbox = new GameObject("AttackHitbox_Investida");
        InvestidaAtaqueInfo infoInvestida = (InvestidaAtaqueInfo)info;

        // Transform
        hitbox.transform.SetParent(atacador.GetAttackHolder().transform);
        hitbox.transform.localPosition = Vector3.zero;
        hitbox.transform.localScale = Vector3.one * infoInvestida.raio;
        hitbox.transform.localEulerAngles = Vector3.zero;

        hitbox.AddComponent<SphereCollider>();

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


    public override void OnUpdate(AtaqueInstance.Estado estado) {
        if (estado == AtaqueInstance.Estado.Hit) {
            GameObject hit = atacador.GetSelf();
            CharacterController cc = hit.GetComponent<CharacterController>();
            cc.Move(hit.transform.forward * velocidade * Time.deltaTime);
        }

       base.OnUpdate(estado);
    }

    public override void OnRecovery() {
        atacador.GetAnimator().SetBool(atacador.AttackTriggerName(), false);
        GameManager.instance.StartCoroutine(UpdateLastLocation());
    }

    IEnumerator UpdateLastLocation() {
        if (atacador.GetSelf().GetComponent<NavMeshAgent>() == null) yield break;

        atacador.GetSelf().GetComponent<NavMeshAgent>().enabled = true;
        yield return null;
        atacador.GetSelf().GetComponent<NavMeshAgent>().enabled = false;
    }

    public override void OnEnd() {
        base.OnEnd();
        animator.applyRootMotion = false;
    }
}
