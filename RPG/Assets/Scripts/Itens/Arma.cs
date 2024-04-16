using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : Equipamento, IAtacador {
    public int dano;
    public float hitboxDuration; // temp
    public GameObject hitbox;

    public Ataque ataque;

    public System.Action onAttackHit;
    
    public override void Equip() {
        Player.instance.EquiparArma(this);
    }

    public override void Unequip() {
        Player.instance.DesequiparArma();
    }

    public virtual void Atacar() {
        ataque.Atacar(this);
    }

    IEnumerator DesativarHitbox() {
        yield return new WaitForSeconds(hitboxDuration);
        hitbox.SetActive(false);
    }

    public Animator GetAnimator() { return Player.instance.animator; }
    public GameObject GetAttackHitboxHolder() { return Player.instance.meio; }
    public string AttackTriggerName() { return "Ataque"; }


    // Quando o ataque da arma colide com um inimigo
    public virtual void OnAtaqueHit(GameObject inimigo) {
        if (!inimigo.CompareTag("Inimigo")) return;

        Player.instance.AumentarCalor(2);
        inimigo.GetComponent<PossuiVida>().LevarDano(ataque.dano);
    }
}
