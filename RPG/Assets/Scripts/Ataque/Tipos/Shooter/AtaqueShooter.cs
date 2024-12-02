using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AtaqueShooter: AttackBehaviour {
    public GameObject projetilPrefab;
    public float tempoDeVida;
    public float velocidade;
    public DamageInfo dano;
    public bool olharEnquantoCarrega = true, autoAim = true;
    Inimigo inimigo;

    GameObject atacadorGO;

    public AtaqueShooter(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        ShooterAtaqueInfo info = (ShooterAtaqueInfo)ataqueInfo;
        projetilPrefab = info.projetilPrefab;
        tempoDeVida = info.tempoDeVida;
        velocidade = info.velocidade;
        dano = info.dano;
        olharEnquantoCarrega = info.olharEnquantoCarrega;
        autoAim = info.autoAim;

        atacadorGO = atacador.GetInfo().gameObject;
    }

    public override void OnEnter() {
        Inimigo inimigo = atacadorGO.GetComponent<Inimigo>();
        if (inimigo != null) {
            this.inimigo = inimigo;
        }
    }

    public override void OnUpdate(AtaqueInstance.Estado estado) {
        if (olharEnquantoCarrega && estado == AtaqueInstance.Estado.Antecipacao && inimigo != null) {
            Vector3 target = inimigo.target.transform.position;
            target.y = atacadorGO.transform.position.y;
            atacadorGO.transform.LookAt(target);
        }
    }

    public override void OnAttack() {
        Transform saida = atacador.GetInfo().attackHolder.transform;

        Vector3 forward = saida.forward;

        if (inimigo != null && autoAim) {
            Transform target = inimigo.target.transform;

            if (target == Player.instance.transform) {
                target = Player.instance.meio.transform;
            }

            forward = (target.position - saida.position).normalized;
        }

        GameObject projetil = GameObject.Instantiate(projetilPrefab, saida.position, saida.rotation);
        projetil.transform.forward = forward;

        Projetil p = projetil.GetComponent<Projetil>();
        if (p != null) {
            p.tempoDeVida = tempoDeVida;
            p.dano = dano.dano;
            p.SetInfoFromOrigem(dano);
            p.velocidade = velocidade;
            p.ignoreList.Add(atacadorGO);
        }
    }

    public override void OnHit(GameObject hit) {}
}
