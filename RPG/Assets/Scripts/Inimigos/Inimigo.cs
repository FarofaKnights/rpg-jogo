using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PossuiVida))]
public class Inimigo : MonoBehaviour, IAtacador {
    [Header("Referência de Componentes")]
    public CharacterController controller;
    public Animator animator;
    public Text vidaText;


    [Header("MISC")]
    public GameObject target;
    public int recompensaPecas = 10;
    public float searchRange = 10f;
    PossuiVida vidaController;
    public GameObject getHitParticles;


    [Header("Configurações de Ataque")]
    public AtaqueInfo ataque;
    [SerializeField] GameObject attackHitboxHolder;
    public GameObject GetAttackHitboxHolder() { return attackHitboxHolder; }
    public Animator GetAnimator() { return animator; }
    public string AttackTriggerName() { return "Attack"; }
    
    

    public StateMachine<IEnemyState> stateMachine;
    public EnemyIdleState idleState;
    public EnemyAttackState attackState;
    public EnemyWalkState walkState;
    public EnemyHittedState hittedState;

    
    string estado_atual = "nenhum";
    

    void Awake() {
        vidaController = GetComponent<PossuiVida>();

        vidaController.onChange += (vida) => {
            float porcentagem = vida / vidaController.VidaMax;
            vidaText.text = (porcentagem * 100).ToString("0") + "%";

            stateMachine.SetState(hittedState);
        };

        vidaController.onDeath += () => {
            Player.instance.AddPecas(recompensaPecas);
        };
    }

    void Start() {
        stateMachine = new StateMachine<IEnemyState>();
        idleState = new EnemyIdleState(this);
        attackState = new EnemyAttackState(this);
        walkState = new EnemyWalkState(this);
        hittedState = new EnemyHittedState(this);

        stateMachine.OnStateChange += (state) => {
            estado_atual = state.ToString();
        };

        stateMachine.SetState(idleState);
    }


    void Update()  {
        stateMachine.Execute();
    }

    // Não sei oque isso faz :P
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ataque")) {
            target = collision.gameObject;
        }
    }

    public void OnAtaqueHit(GameObject other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PossuiVida>().LevarDano(ataque.dano);
        }
    }


    // draw gizmos
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }

    public void SpawnParticle()
    {
        GameObject particle = Instantiate(getHitParticles, attackHitboxHolder.transform.position, transform.rotation);
        particle.transform.SetParent(this.gameObject.transform);
    }
}
