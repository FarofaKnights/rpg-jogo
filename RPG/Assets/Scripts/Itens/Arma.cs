using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : Equipamento {
    public int dano;
    public float hitboxDuration; // temp
    public GameObject hitbox;

    public System.Action onAttackHit;
    
    public override void Equip() {
        Player.instance.EquiparArma(this);
        hitbox.SetActive(false);
        hitbox.GetComponent<OnTrigger>().onTriggerEnter += OnHit;
    }

    public override void Unequip() {
        Player.instance.DesequiparArma();
        hitbox.SetActive(false);
        hitbox.GetComponent<OnTrigger>().onTriggerEnter -= OnHit;
    }

    public virtual void Atacar() {
        hitbox.SetActive(false);
        hitbox.SetActive(true);

        // Após um tempo, desativar a hitbox
        StartCoroutine(DesativarHitbox());
    }

    IEnumerator DesativarHitbox() {
        yield return new WaitForSeconds(hitboxDuration);
        hitbox.SetActive(false);
    }

    // Quando o ataque da arma colide com um inimigo
    public virtual void OnHit(GameObject inimigo) {
        Player.instance.AumentarCalor(2);
        inimigo.GetComponent<PossuiVida>().LevarDano(dano);
    }
}
