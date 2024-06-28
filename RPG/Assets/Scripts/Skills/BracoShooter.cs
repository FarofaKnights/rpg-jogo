using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracoShooter : Braco {
    public Transform saida;
    public GameObject projetilPrefab;

    public float tempoDeVida = 2;
    public float dano = 1;
    public float velocidade = 10;

    protected override void AtivarEfeito() {
        GameObject projetil = Instantiate(projetilPrefab, saida.position, saida.rotation);
        Vector3 playerForward = Player.instance.transform.forward;
        projetil.transform.forward = playerForward;

        Projetil p = projetil.GetComponent<Projetil>();
        p.tempoDeVida = tempoDeVida;
        p.dano = dano;
        p.velocidade = velocidade;

        Player.instance.animator.SetTrigger("Atirar");
    }
}
