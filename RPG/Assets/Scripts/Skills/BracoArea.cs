using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracoArea : Braco {
    public GameObject areaPrefab;
    public float tempoDeVida = 1;
    public float dano = 1;
    public float scaleFinal = 1;

    protected override void AtivarEfeito() {
        Transform playerFoot = Player.instance.pe;
        GameObject obj = Instantiate(areaPrefab, playerFoot.transform.position, Quaternion.identity);

        SkillArea area = obj.GetComponent<SkillArea>();
        area.tempoDeVida = tempoDeVida;
        area.dano = dano;
        area.scaleFinal = scaleFinal;
    }
}
