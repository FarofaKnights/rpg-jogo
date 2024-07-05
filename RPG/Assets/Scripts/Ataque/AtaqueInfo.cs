using UnityEngine;

public abstract class AtaqueInfo : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Dano")]
    public int dano;
    
    [Header("Timing")]
    public bool usarTiming = true;
    public int antecipacaoFrames;
    public int hitFrames;
    public int recoveryFrames;
    public int cancelFrames = -1;

    [Header("Movimento")]
    public bool moveDuranteAtaque = false;
    public int moveStartFrame;
    public int moveEndFrame;
    public float moveDistance;

    public abstract AtaqueInstance Atacar(IAtacador atacador);
}