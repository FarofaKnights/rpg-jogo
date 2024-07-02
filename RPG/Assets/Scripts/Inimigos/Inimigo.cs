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


    [Header("Configurações IA")]
    public float rangeProcurando = 5f; // Range que sai do Idle pro Walk
    public float rangePerderTarget = 15f; // Range que sai do Walk pro Idle
    public float minRangeProximidade = 1.25f; // Range que sai do Walk pro Attack
    public float maxRangeProximidade = 0.75f; // Range que sai do Attack pro Walk
    public bool precisaDeVisaoDireta = false; // Se precisa de visão para atacar
    public float tomouDanoStun = 1f; // Tempo que fica parado ao tomar dano
    public TriggerMode modoDeTriggerDeAnimacao = TriggerMode.Trigger;


    [Header("MISC")]
    public GameObject target;
    public int recompensaPecas = 10;
    PossuiVida vidaController;
    public GameObject getHitParticles;
    public AudioSource AlertSound;
    public AudioSource attackSound;
    public bool detectado = false;


    [Header("Configurações de Ataque")]
    public AtaqueInfo ataque;
    [SerializeField] GameObject attackHitboxHolder;
    public GameObject GetAttackHolder() { return attackHitboxHolder; }
    public Animator GetAnimator() { return animator; }
    public string AttackTriggerName() { return "Attack"; }
    public GameObject GetSelf() { return gameObject; }
    public TriggerMode GetTriggerMode() { return modoDeTriggerDeAnimacao; }

    
    

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
            Player.Atributos.pecas.Add(recompensaPecas);
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

    public void OnAtaqueHit(GameObject other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PossuiVida>().LevarDano(ataque.dano);
        }
    }


    // draw gizmos
    void OnDrawGizmosSelected() {
        if (stateMachine == null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangeProcurando);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, minRangeProximidade);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, rangePerderTarget);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRangeProximidade);
        } else if (stateMachine.GetCurrentState() == idleState) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangeProcurando);
        } else if (stateMachine.GetCurrentState() == walkState) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.transform.position, minRangeProximidade);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.transform.position, rangePerderTarget);
        } else if (stateMachine.GetCurrentState() == attackState) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.transform.position, maxRangeProximidade);
        }
      
        
    }

    public void SpawnParticle()
    {
        GameObject particle = Instantiate(getHitParticles, attackHitboxHolder.transform.position, transform.rotation);
        particle.transform.SetParent(this.gameObject.transform);
    }

    public void Morrer() {
        AudioManager.instance.enemyDeath.Play();
        SpawnParticle();
        Destroy(gameObject);
    }
}
