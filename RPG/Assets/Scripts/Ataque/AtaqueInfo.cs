using UnityEngine;

public abstract class AtaqueInfo : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Dano")]
    public int dano;
    
    [Header("Timing")]
    public int antecipacaoFrames;
    public int hitFrames;
    public int recoveryFrames;

    public abstract AtaqueInstance Atacar(IAtacador atacador);
}