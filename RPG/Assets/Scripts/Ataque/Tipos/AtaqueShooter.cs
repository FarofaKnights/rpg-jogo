using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AtaqueShooter: AtaqueInstance {
    public GameObject projetilPrefab;
    public float tempoDeVida;
    public float velocidade;
    public int dano;

    public AtaqueShooter(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) {
        ShooterAtaqueInfo info = (ShooterAtaqueInfo)ataqueInfo;
        projetilPrefab = info.projetilPrefab;
        tempoDeVida = info.tempoDeVida;
        velocidade = info.velocidade;
        dano = info.dano;
    }

    public override void OnAttack() {
        Transform saida = atacador.GetAttackHolder().transform;

        GameObject projetil = GameObject.Instantiate(projetilPrefab, saida.position, saida.rotation);
        projetil.transform.forward = saida.forward;

        Projetil p = projetil.GetComponent<Projetil>();
        p.tempoDeVida = tempoDeVida;
        p.dano = dano;
        p.velocidade = velocidade;
    }
}
