using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo AtaqueInfo", menuName = "RPG/MeleeInfo")]
public class MeleeAtaqueInfo : AtaqueInfo {

    [Header("Configurações do Hitbox")]
    public Vector3 hitboxOffset;
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxRotation;

    public override AtaqueInstance Atacar(IAtacador atacador) {
        return MeleeAtaqueInfo.Atacar(this, atacador);
    }

    public static AtaqueInstance Atacar(MeleeAtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueMelee(AtaqueInfo, atacador);
    }

}