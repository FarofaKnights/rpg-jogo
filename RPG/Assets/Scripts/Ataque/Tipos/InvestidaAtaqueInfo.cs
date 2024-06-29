using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo InvestidaInfo", menuName = "RPG/InvestidaInfo")]
public class InvestidaAtaqueInfo : AtaqueInfo {
    public float velocidade;
    public float raio;
    public int dano;

    public override AtaqueInstance Atacar(IAtacador atacador) {
        return InvestidaAtaqueInfo.Atacar(this, atacador);
    }

    public static AtaqueInstance Atacar(InvestidaAtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueInvestida(AtaqueInfo, atacador);
    }
}
