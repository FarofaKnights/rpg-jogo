using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Arma : Equipamento, IAtacador {
    public ParticleSystem ps, longPs;
    public MeleeAtaqueInfo ataque;
    public MeleeAtaqueInfo[] ataques;
    public int ataqueIndex = 0;

    public System.Action onAttackHit, onAttackEnd;
    
    public virtual AtaqueInstance Atacar(int index = -1) {
        if (index != -1) ataqueIndex = index;
        if (index >= ataques.Length) return null;
        
        ataque = ataques[ataqueIndex];
        if(ataqueIndex == 2 && longPs != null) longPs.Play();
        if (ps != null) ps.Play();

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

    public void MoveWithAttack(float step, float progress) { equipador.MoveWithAttack(step, progress); }

    public AtacadorInfo GetInfo() {
        return equipador.GetInfo();
    }

    // Quando o ataque da arma colide com um inimigo
    public virtual bool OnAtaqueHit(GameObject inimigo) {
        bool res = equipador.OnAtaqueHit(inimigo);
        if (res) onAttackHit?.Invoke();
        return res;
    }
}
