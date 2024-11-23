using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo AtaqueInfo", menuName = "RPG/MeleeInfo")]
public class MeleeAtaqueInfo : AtaqueInfo {
    public enum TipoHitbox { Box, Sphere }

    [Header("Configurações do Hitbox")]
    public TipoHitbox tipoHitbox = TipoHitbox.Box;
    public Vector3 hitboxOffset;

    [Header("Hitbox Box")]
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxRotation;

    [Header("Hitbox Sphere")]
    public float hitboxRadius = 1;

    public override AtaqueInstance Atacar(IAtacador atacador) {
        return MeleeAtaqueInfo.Atacar(this, atacador);
    }

    public override AttackBehaviour GetBehaviour(IAtacador atacador) {
        return new AtaqueMelee(this, atacador);
    }

    public static AtaqueInstance Atacar(MeleeAtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueInstance(AtaqueInfo, atacador);
    }

}