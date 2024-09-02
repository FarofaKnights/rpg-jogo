using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo InvestidaInfo", menuName = "RPG/InvestidaInfo")]
public class InvestidaAtaqueInfo : AtaqueInfo {
    public float velocidade;
    public float raio;
    

    [Header("Configurações Especificas")]
    public bool pararAoAcertar = true;
    public int cooldownMaiorACada = -1;
    public float cooldownMaiorACadaValor = 0;

    public override AtaqueInstance Atacar(IAtacador atacador) {
        return InvestidaAtaqueInfo.Atacar(this, atacador);
    }

    public override AttackBehaviour GetBehaviour(IAtacador atacador) {
        return new AtaqueInvestida(this, atacador);
    }
    
    public static AtaqueInstance Atacar(InvestidaAtaqueInfo ataqueInfo, IAtacador atacador) {
        return new AtaqueInstance(ataqueInfo, atacador);
    }
}
