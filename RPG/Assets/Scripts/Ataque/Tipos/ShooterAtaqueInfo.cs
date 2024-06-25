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

    public static AtaqueInstance Atacar(ShooterAtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueShooter(AtaqueInfo, atacador);
    }

}