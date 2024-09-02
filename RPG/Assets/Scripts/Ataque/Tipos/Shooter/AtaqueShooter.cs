using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AtaqueShooter: AttackBehaviour {
    public GameObject projetilPrefab;
    public float tempoDeVida;
    public float velocidade;
    public int dano;
    public bool olharEnquantoCarrega = true, autoAim = true;
    Inimigo inimigo;

    public AtaqueShooter(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        ShooterAtaqueInfo info = (ShooterAtaqueInfo)ataqueInfo;
        projetilPrefab = info.projetilPrefab;
        tempoDeVida = info.tempoDeVida;
        velocidade = info.velocidade;
        dano = info.dano;
        olharEnquantoCarrega = info.olharEnquantoCarrega;
        autoAim = info.autoAim;
    }

    public override void OnEnter() {
        Inimigo inimigo = atacador.GetSelf().GetComponent<Inimigo>();
        if (inimigo != null) {
            this.inimigo = inimigo;
        }
    }

    public override void OnUpdate(AtaqueInstance.Estado estado) {
        if (olharEnquantoCarrega && estado == AtaqueInstance.Estado.Antecipacao && inimigo != null) {
            Vector3 target = inimigo.target.transform.position;
            target.y = atacador.GetSelf().transform.position.y;
            atacador.GetSelf().transform.LookAt(target);
        }
    }

    public override void OnAttack() {
        Transform saida = atacador.GetAttackHolder().transform;

        Vector3 forward = saida.forward;

        if (inimigo != null && autoAim) {
            Transform target = inimigo.target.transform;
            forward = (target.position - saida.position).normalized;
            forward.y = 0;
        }

        GameObject projetil = GameObject.Instantiate(projetilPrefab, saida.position, saida.rotation);
        projetil.transform.forward = forward;

        Projetil p = projetil.GetComponent<Projetil>();
        if (p != null) {
            p.tempoDeVida = tempoDeVida;
            p.dano = dano;
            p.velocidade = velocidade;
            p.ignoreList.Add(atacador.GetSelf());
        }
    }

    public override void OnHit(GameObject hit) {}
}
