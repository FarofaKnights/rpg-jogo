using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo AtaqueInfo", menuName = "RPG/AtaqueInfo")]
public class AtaqueInfo : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Hitbox")]
    public Vector3 hitboxOffset;
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxRotation;
    public float hitboxDelay;
    public float hitboxDuration;

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
    public System.Action onHitFinish;
    public StateMachine stateMachine;

    public AntecipacaoState antecipacaoState;
    public HitState hitState;
    public RecoveryState recoveryState;

    bool entrouNoEstado = false;

    public AtaqueInstance(AtaqueInfo info, IAtacador atacador) {
        this.info = info;
        this.atacador = atacador;
        animator = atacador.GetAnimator();
        

        hitbox = CreateHitbox();
        hitbox.SetActive(false);
        hitbox.GetComponent<OnTrigger>().onTriggerEnter += (GameObject hit) => {
            atacador.OnAtaqueHit(hit);
        };

        animator.runtimeAnimatorController = info.animatorOverride;
        animator.applyRootMotion = true;
        animator.SetTrigger(atacador.AttackTriggerName());

        hitbox.SetActive(true);
        
        coroutineHitbox = DesativarHitbox(info.hitboxDuration);
        coroutineAnimation = Update();
        GameManager.instance.StartCoroutine(coroutineHitbox);
        GameManager.instance.StartCoroutine(coroutineAnimation);
    }

    GameObject CreateHitbox() {
        GameObject hitbox = new GameObject("AttackHitbox");
        hitbox.transform.SetParent(atacador.GetAttackHitboxHolder().transform);
        hitbox.transform.localPosition = info.hitboxOffset;
        hitbox.transform.localScale = info.hitboxSize;
        hitbox.transform.localEulerAngles = info.hitboxRotation;
        hitbox.AddComponent<BoxCollider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>();
        return hitbox;
    }

    IEnumerator DesativarHitbox(float hitboxDuration) {
        yield return new WaitForSeconds(hitboxDuration);
        Parar();
    }

    IEnumerator Update() {
        while (true) {
            if (animator == null) yield break;
            if (!entrouNoEstado && animator.GetCurrentAnimatorStateInfo(0).IsName("AtaquePlayer")) {
                entrouNoEstado = true;
                float animationTime = animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animationTime);
                OnAnimacaoAcabou();
                yield break;
            }
            yield return null;
        }
    }

    void OnAnimacaoAcabou() {
        Parar();
        animator.applyRootMotion = false;
        GameManager.instance.StopCoroutine(coroutineHitbox);
        if (onHitFinish != null) onHitFinish();
    }

    public void Interromper() {
        Parar();
        GameManager.instance.StopCoroutine(coroutineHitbox);
        GameManager.instance.StopCoroutine(coroutineAnimation);
        if (onHitFinish != null) onHitFinish();
    }

    void Parar() {
        if (hitbox == null) return;
        
        hitbox.SetActive(false);
        Object.Destroy(hitbox);
    }
}