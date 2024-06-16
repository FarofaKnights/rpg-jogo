using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo AtaqueInfo", menuName = "RPG/AtaqueInfo")]
public class AtaqueInfo : ScriptableObject {
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
        return AtaqueInfo.Atacar(this, atacador);
    }

    public static AtaqueInstance Atacar(AtaqueInfo AtaqueInfo, IAtacador atacador) {
        return new AtaqueInstance(AtaqueInfo, atacador);
    }

}

public class AtaqueInstance {
    public AtaqueInfo info;
    public IAtacador atacador;
    public GameObject hitbox;
    IEnumerator coroutineHitbox, coroutineAnimation;
    Animator animator;
    public System.Action onEnd;


    public StateMachine<IAtaqueState> stateMachine;
    public AntecipacaoState antecipacaoState;
    public HitState hitState;
    public RecoveryState recoveryState;
    public AtaqueEndState endState;


    public AtaqueInstance(AtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        animator = atacador.GetAnimator();

        stateMachine = new StateMachine<IAtaqueState>();
        antecipacaoState = new AntecipacaoState(this);
        hitState = new HitState(this);
        recoveryState = new RecoveryState(this);
        endState = new AtaqueEndState(this);
        

        CreateHitbox();

        animator.runtimeAnimatorController = info.animatorOverride;
        animator.applyRootMotion = false;
        animator.SetTrigger(atacador.AttackTriggerName());


        stateMachine.SetState(antecipacaoState);
    }

    public void Update() {
        stateMachine.Execute();
    }

    protected void CreateHitbox() {
        hitbox = new GameObject("AttackHitbox");

        // Transform
        hitbox.transform.SetParent(atacador.GetAttackHitboxHolder().transform);
        hitbox.transform.localPosition = info.hitboxOffset;
        hitbox.transform.localScale = info.hitboxSize;
        hitbox.transform.localEulerAngles = info.hitboxRotation;

        // Trigger
        hitbox.AddComponent<BoxCollider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>().onTriggerEnter += (GameObject hit) => {
            atacador.OnAtaqueHit(hit);
            GameManager.instance.StartCoroutine(SlowdownOnHit());
        };

        hitbox.SetActive(false);
    }

    IEnumerator SlowdownOnHit() {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.05f);
        if (!GameManager.instance.IsPaused()) Time.timeScale = 1;
    }

    public void AtivarHitbox() {
        hitbox.SetActive(true);
    }

    public void DesativarHitbox() {
        hitbox.SetActive(false);
    }

    public void HandleAtaqueEnd() {
        Parar();
        animator.applyRootMotion = false;
        if (onEnd != null) onEnd();
    }

    public void Interromper() {
        Parar();
        if (onEnd != null) onEnd();
    }

    void Parar() {
        if (hitbox == null) return;
        
        hitbox.SetActive(false);
        Object.Destroy(hitbox);
    }
}