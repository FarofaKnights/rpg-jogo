using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracoShooter : Braco {
    public Transform saida;
    public GameObject projetilPrefab;
    public LayerMask ignoreProjectileMask;

    public float tempoDeVida = 2;
    public float dano = 1;
    DamageInfo danoInfo;
    public float velocidade = 10;

    public List<string> ignoreTags = new List<string>() { "Player", "Arma" };
    public string triggerName = "Atirar";

    protected override void AtivarEfeito() {
        AtacadorInfo atacadorInfo = equipador.GetInfo();

        GameObject projetil = Instantiate(projetilPrefab, saida.position, saida.rotation);
        Vector3 playerForward = atacadorInfo.gameObject.transform.forward;

        if (atacadorInfo.gameObject == Player.instance.gameObject) {
            Vector3 aimDirection = GetAimDirection();
            projetil.transform.forward = aimDirection;
        } else {
            projetil.transform.forward = playerForward;
        }

        atacadorInfo.animator.SetTrigger(triggerName);

        Projetil p = projetil.GetComponent<Projetil>();
        p.ignoreTags = ignoreTags;
        p.tempoDeVida = tempoDeVida;
        p.dano = dano;
        p.velocidade = velocidade;
        p.SetInfoFromOrigem(GetDano());
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

    public float GetMaxDistance() {
        return tempoDeVida * velocidade;
    }

    public override DamageInfo GetDano() {
        if (danoInfo == null) {
            danoInfo = new DamageInfo {
                tipoDeDano = TipoDeDano.Projetil,
                formaDeDano = FormaDeDano.Ativo,
                dano = dano,
                origem = equipador.GetInfo().gameObject
            };
        }
        return danoInfo;
    }
}
