using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo Ataque Shooter", menuName = "RPG/ShooterInfo")]
public class ShooterAtaqueInfo : AtaqueInfo {

    [Header("Configurações do Tiro")]
    public GameObject projetilPrefab;

    public float tempoDeVida = 2;
    public float velocidade = 10;

    public override AtaqueInstance Atacar(IAtacador atacador) {
        return ShooterAtaqueInfo.Atacar(this, atacador);
    }

    public override AttackBehaviour GetBehaviour(IAtacador atacador) {
        return new AtaqueShooter(this, atacador);
    }

    public static AtaqueInstance Atacar(ShooterAtaqueInfo ataqueInfo, IAtacador atacador) {
        return new AtaqueInstance(ataqueInfo, atacador);
    }

}