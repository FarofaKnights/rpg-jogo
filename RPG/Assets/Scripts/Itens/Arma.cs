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

    public void MoveWithAttack(float step, float progress) { Player.instance. MoveWithAttack(step, progress); }

    public AtacadorInfo GetInfo() {
        return Player.instance.GetInfo();
    }

    // Quando o ataque da arma colide com um inimigo
    public virtual void OnAtaqueHit(GameObject inimigo) {
        if (!inimigo.CompareTag("Inimigo")) return;

        Player.Atributos.calor.Add(10);
        float adicional = Player.Stats.GetAdicionalForca(ataque.dano);
        if (inimigo.GetComponent<Inimigo>() != null)
            inimigo.GetComponent<Inimigo>().hittedDir = ataqueIndex;
        
        if (inimigo.GetComponent<PossuiVida>() != null)
            inimigo.GetComponent<PossuiVida>().LevarDano(ataque.dano + adicional);
        else if (inimigo.GetComponentInChildren<PossuiVida>() != null)
            inimigo.GetComponentInChildren<PossuiVida>().LevarDano(ataque.dano + adicional);
    }
}
