using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Novo Ataque", menuName = "RPG/Ataque")]
public class Ataque : ScriptableObject {
    public AnimatorOverrideController animatorOverride;

    [Header("Configurações do Hitbox")]
    public Vector3 hitboxOffset;
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxRotation;
    public float hitboxDelay;
    public float hitboxDuration;

    [Header("Configurações do Dano")]
    public int dano;

    public AtaqueInstance Atacar(IAtacador atacador) {
        return Ataque.Atacar(this, atacador);
    }

    public static AtaqueInstance Atacar(Ataque ataque, IAtacador atacador) {
        return new AtaqueInstance(ataque, atacador);
    }

}

public class AtaqueInstance {
    public Ataque ataque;
    public IAtacador atacador;
    public GameObject hitbox;
    IEnumerator coroutineHitbox, coroutineAnimation;
    Animator animator;
    public System.Action onHitFinish;

    public AtaqueInstance(Ataque ataque, IAtacador atacador) {
        this.ataque = ataque;
        this.atacador = atacador;
        animator = atacador.GetAnimator();

        hitbox = CreateHitbox();
        hitbox.SetActive(false);
        hitbox.GetComponent<OnTrigger>().onTriggerEnter += (GameObject hit) => {
            atacador.OnAtaqueHit(hit);
        };

        animator.runtimeAnimatorController = ataque.animatorOverride;
        animator.SetTrigger("Attack");

        hitbox.SetActive(true);
        
        coroutineHitbox = DesativarHitbox(ataque.hitboxDuration);
        coroutineAnimation = AnimacaoAcabou();
        GameManager.instance.StartCoroutine(coroutineHitbox);
        GameManager.instance.StartCoroutine(coroutineAnimation);
    }

    GameObject CreateHitbox() {
        GameObject hitbox = new GameObject("AttackHitbox");
        hitbox.transform.SetParent(atacador.GetAttackHitboxHolder().transform);
        hitbox.transform.localPosition = ataque.hitboxOffset;
        hitbox.transform.localScale = ataque.hitboxSize;
        hitbox.transform.localEulerAngles = ataque.hitboxRotation;
        hitbox.AddComponent<BoxCollider>().isTrigger = true;
        hitbox.AddComponent<OnTrigger>();
        return hitbox;
    }

    IEnumerator DesativarHitbox(float hitboxDuration) {
        yield return new WaitForSeconds(hitboxDuration);
        Parar();
    }

    IEnumerator AnimacaoAcabou() {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Parar();
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