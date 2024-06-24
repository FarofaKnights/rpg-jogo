using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo AtaqueInfo", menuName = "RPG/AtaqueInfo")]
public class MeleeAtaqueInfo : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Hitbox")]
    public Vector3 hitboxOffset;
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxRotation;

    [Header("Timing")]
    public int antecipacaoFrames;
    public int hitFrames;
    public int recoveryFrames;

    [Header("Configurações do Dano")]
    public int dano;

    public AtaqueInstance Atacar(IAtacador atacador) {
        return MeleeAtaqueInfo.Atacar(this, atacador);
    }

    public static AtaqueInstance Atacar(MeleeAtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueMelee(AtaqueInfo, atacador);
    }

}