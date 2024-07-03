using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Arma : Equipamento, IAtacador {
    public ParticleSystem ps, longPs;
    public MeleeAtaqueInfo ataque;
    public MeleeAtaqueInfo[] ataques;
    int ataqueIndex = 0;

    public System.Action onAttackHit, onAttackEnd;
    
    public override void Equip() {
        Player.Inventario.EquiparArma(this);
    }

    public override void Unequip() {
        Player.Inventario.DesequiparArma();
    }

    public virtual AtaqueInstance Atacar(int index = -1) {
        if (index != -1) ataqueIndex = index;
        if (index >= ataques.Length) return null;
        
        ataque = ataques[ataqueIndex];
        if(ataqueIndex == 2)
        {
            longPs.Play();
        }
        ps.Play();
        AtaqueInstance ataqueInstance = ataque.Atacar(this);
        ataqueInstance.onEnd += () => {
            onAttackEnd?.Invoke();
        };
        ataqueIndex = (ataqueIndex + 1) % ataques.Length;

        return ataqueInstance;
    }

    public void Resetar() {
        ataqueIndex = 0;
    }

    public Animator GetAnimator() { return Player.instance.animator; }
    public GameObject GetAttackHolder() { return Player.instance.meio; }
    public string AttackTriggerName() { return "Ataque"; }

    public GameObject GetSelf() { return gameObject; }
    public TriggerMode GetTriggerMode() { return TriggerMode.Trigger; }
    public void MoveWithAttack(float step, float progress) { Player.instance. MoveWithAttack(step, progress); }


    // Quando o ataque da arma colide com um inimigo
    public virtual void OnAtaqueHit(GameObject inimigo) {
        if (!inimigo.CompareTag("Inimigo")) return;

        Player.Atributos.calor.Add(10);
        float adicional = Player.Stats.GetAdicionalForca(ataque.dano);
        inimigo.GetComponent<PossuiVida>().LevarDano(ataque.dano + adicional);
    }
}
