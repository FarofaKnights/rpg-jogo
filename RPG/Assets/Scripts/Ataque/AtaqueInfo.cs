using UnityEngine;

public enum TipoInfligirDano {
    Unico, Continuo, PorAnimacao, Projetil
}

public abstract class AtaqueInfo : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Dano")]
    public int dano;
    public TipoInfligirDano tipoInfligirDano = TipoInfligirDano.Unico;
    public float frequenciaDeDano = 0;
    
    [Header("Timing")]
    public bool usarTiming = true;
    public int antecipacaoFrames;
    public int hitFrames;
    public int recoveryFrames;
    public int cancelFrames = -1;

    [Header("Movimento")]
    public bool moveDuranteAtaque = false;
    public bool moveWithTiming = true;
    public int moveStartFrame;
    public int moveEndFrame;
    public float moveDistance;
    public bool moveWithTrigger = false;
    public float moveOnTriggerSpeed = 1;

    public abstract AtaqueInstance Atacar(IAtacador atacador);
    public abstract AttackBehaviour GetBehaviour(IAtacador atacador);
    public HitListener GetHitListener(AttackBehaviour ataque) {
        switch (tipoInfligirDano) {
            case TipoInfligirDano.Unico:
                return new AtaqueUnico(ataque);
            case TipoInfligirDano.Continuo:
                return new AtaqueContinuo(ataque);
            case TipoInfligirDano.PorAnimacao:
                return new AtaqueAnimacao(ataque);
            case TipoInfligirDano.Projetil:
                return new AtaqueProjetil(ataque);
            default:
                return null;
        }
    }
}