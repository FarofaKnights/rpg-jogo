using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : Equipamento, IAtacador {
    public int dano;
    public float hitboxDuration; // temp
    public GameObject hitbox;

    public AtaqueInfo ataque;
    public AtaqueInfo[] ataques;
    int ataqueIndex = 0;

    public System.Action onAttackHit, onAttackEnd;
    
    public override void Equip() {
        Player.instance.EquiparArma(this);
    }

    public override void Unequip() {
        Player.instance.DesequiparArma();
    }

    public virtual void Atacar(int index = -1) {
        if (index != -1) ataqueIndex = index;
        ataque = ataques[ataqueIndex];
        ataque.Atacar(this).onHitFinish += () => {
            onAttackEnd?.Invoke();
        };
        ataqueIndex = (ataqueIndex + 1) % ataques.Length;
    }

    public void Resetar() {
        ataqueIndex = 0;
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
