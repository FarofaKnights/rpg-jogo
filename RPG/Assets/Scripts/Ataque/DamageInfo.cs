using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDanoIndireto {
    void SetInfoFromOrigem(DamageInfo damageInfo);
}

public enum TipoDeDano { Misterio, Melee, Area, Projetil, Explosao}
public enum FormaDeDano { Misterio, Ativo, Passivo }

[System.Serializable]
public class DamageInfo {
    public TipoDeDano tipoDeDano;
    public FormaDeDano formaDeDano;
    public float dano;
    [HideInInspector] public float danoAdicional = 0;
    [HideInInspector] public GameObject origem;

    public DamageInfo() { }

    public DamageInfo(DamageInfo info) {
        tipoDeDano = info.tipoDeDano;
        formaDeDano = info.formaDeDano;
        dano = info.dano;
        origem = info.origem;
        danoAdicional = info.danoAdicional;
    }
}
