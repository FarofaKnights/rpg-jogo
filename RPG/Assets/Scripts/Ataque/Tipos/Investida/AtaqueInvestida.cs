using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtaqueInvestida : HitboxAttackBehaviour {
    public float velocidade;
    public int dano;
    InvestidaAtaqueInfo infoInvestida;
    bool canWalk = true;

    public AtaqueInvestida(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        infoInvestida = (InvestidaAtaqueInfo)ataqueInfo;
        velocidade = infoInvestida.velocidade;
        dano = infoInvestida.dano;
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
        if (infoInvestida.pararAoAcertar) canWalk = false;
        atacador.OnAtaqueHit(hit);
        GameManager.instance.SlowdownOnHit();
    }


    public override void OnUpdate(AtaqueInstance.Estado estado) {
        if (canWalk && estado == AtaqueInstance.Estado.Hit) {
            GameObject hit = atacador.GetSelf();
            CharacterController cc = hit.GetComponent<CharacterController>();
            Vector3 move = hit.transform.forward * velocidade * Time.fixedDeltaTime;
            
            if (!cc.isGrounded) move.y = -9.8f * Time.fixedDeltaTime;
                
            cc.Move(move);
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
