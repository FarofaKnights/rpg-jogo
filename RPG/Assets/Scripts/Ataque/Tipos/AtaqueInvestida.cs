using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtaqueInvestida : AtaqueInstance {
    public float velocidade;
    public int dano;
    public GameObject hitbox;
    List<GameObject> hits = new List<GameObject>();


    public AtaqueInvestida(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        InvestidaAtaqueInfo info = (InvestidaAtaqueInfo)ataqueInfo;
        velocidade = info.velocidade;
        dano = info.dano;

        CreateHitbox();
    }

    protected void CreateHitbox() {
        hitbox = new GameObject("AttackHitbox");
        InvestidaAtaqueInfo infoMelee = (InvestidaAtaqueInfo)info;

        // Transform
        hitbox.transform.SetParent(atacador.GetAttackHolder().transform);
        hitbox.transform.localPosition = Vector3.zero;
        hitbox.transform.localScale = Vector3.one * infoMelee.raio;
        hitbox.transform.localEulerAngles = Vector3.zero;

        // Trigger
        hitbox.AddComponent<SphereCollider>().isTrigger = true;
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

    public override void OnAttack() {
        hitbox.SetActive(true);
    }


    public override void OnUpdate(AtaqueInstance.Estado estado) {
        if (estadoAtual == AtaqueInstance.Estado.Hit) {
            GameObject hit = atacador.GetSelf();
            CharacterController cc = hit.GetComponent<CharacterController>();
            cc.Move(hit.transform.forward * velocidade * Time.deltaTime);
        }
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
        if (hitbox != null) {
            hitbox.SetActive(false);
            Object.Destroy(hitbox);
        }
    
        animator.applyRootMotion = false;
    }
}
