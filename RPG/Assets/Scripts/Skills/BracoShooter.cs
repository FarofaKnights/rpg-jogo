using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracoShooter : Braco {
    public Transform saida;
    public GameObject projetilPrefab;
    public LayerMask ignoreProjectileMask;

    public float tempoDeVida = 2;
    public float dano = 1;
    public float velocidade = 10;

    protected override void AtivarEfeito() {
        // saida = Player.instance.meio.transform;
        GameObject projetil = Instantiate(projetilPrefab, saida.position, saida.rotation);
        Vector3 playerForward = Player.instance.transform.forward;
        Vector3 aimDirection = GetAimDirection();
        projetil.transform.forward = aimDirection;

        Projetil p = projetil.GetComponent<Projetil>();
        p.ignoreTags.Add("Player");
        p.ignoreTags.Add("Arma");
        p.tempoDeVida = tempoDeVida;
        p.dano = dano;
        p.velocidade = velocidade;

        Player.instance.animator.SetTrigger("Atirar");
    }

    public Vector3 GetAimDirection() {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ignoreProjectileMask)) {
            return (hit.point - saida.position).normalized;
        } else {
            return ray.direction;
        }
    }
}
