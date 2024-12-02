using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum TipoArma {
    Leve, Pesada
}

public class Arma : Equipamento, IAtacador {
    public TipoArma tipo = TipoArma.Leve;
    public ParticleSystem ps, longPs;
    public AtaqueInfo ataque;
    public AtaqueInfo[] ataques;
    public int ataqueIndex = 0;
    DamageInfo danoInfo;

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

    public int GetTipoID() {
        switch (tipo) {
            case TipoArma.Leve: return 1;
            case TipoArma.Pesada: return 2;
            default: return 0;
        }
    }

    public virtual DamageInfo GetDano() {
        if (danoInfo == null) {
            danoInfo = new DamageInfo(ataque.dano);
            danoInfo.origem = equipador.GetInfo().gameObject;
        }

        return danoInfo;
    }
}
