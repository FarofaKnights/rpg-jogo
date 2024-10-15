using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct InimigoIADebug {
    public string estado_atual;
    public float distancia_alvo;
    public float dot_prod_olhando;
    public string walk_debug;
    public string attack_debug;
}



[RequireComponent(typeof(PossuiVida))]
public class Inimigo : MonoBehaviour, IAtacador {
    [Header("Referência de Componentes")]
    public CharacterController controller;
    public Animator animator;
    public Text vidaText;
    public GameObject meio;

    [Header("Configurações IA")]
    public float maxYProcurando = 0.5f; // Altura máxima que o inimigo procura
    public float rangeProcurando = 5f; // Range que sai do Idle pro Walk
    public float rangePerderTarget = 15f; // Range que sai do Walk pro Idle
    public float minRangeProximidade = 1.25f; // Range que sai do Walk pro Attack
    public float maxRangeProximidade = 0.75f; // Range que sai do Attack pro Walk
    public bool precisaDeVisaoDireta = false; // Se precisa de visão para atacar
    [Range(0.9f, 1f)] public float precisaoDaVisao = 0.99f; // Precisão da visão
    public float tempoAntesDoAtaque = 0.25f; // Tempo antes de atacar
    public float tomouDanoStun = 1f; // Tempo que fica parado ao tomar dano
    public float empurradoAoSofrerHit = 0.75f;
    public TriggerMode modoDeTriggerDeAnimacao = TriggerMode.Trigger;
    public InimigoIADebug debug;
    public int descansoMaiorAposXAtaques = -1;
    public float multDescansoMaior = 2f;
    int stunHits = 0;
    public int maxStunHits = 3;
    float semStunTimer = 0;
    public float semStunTime = 2.5f;
    public bool IsSemStun { get { return semStunTimer > 0; } }
    [HideInInspector] public int ataquesFeitos = 0;


    [Header("MISC")]
    public GameObject target;
    public int recompensaPecas = 10;
    PossuiVida vidaController;
    public GameObject getHitParticles;
    public AudioSource AlertSound;
    public AudioSource attackSound;
    public bool detectado = false;
    public System.Action OnDestroied; // Diferente do OnDeath, esse é chamado quando o inimigo é destruído, isso tbm conta destruição por integridade (foi morto em outro save e é destruido para não ser carregado)


    [Header("Configurações de Ataque")]
    public AtaqueInfo ataque;
    [SerializeField] GameObject attackHitboxHolder;
    AtacadorInfo atacadorInfo;
    

    public StateMachine<IEnemyState> stateMachine;
    public EnemyIdleState idleState;
    public EnemyAttackState attackState;
    public EnemyWalkState walkState;
    public EnemyHittedState hittedState;

    public int hittedDir = 0;

    
    string estado_atual = "nenhum";
    

    void Awake() {
        vidaController = GetComponent<PossuiVida>();

        vidaController.onChange += (vida) => {
            float porcentagem = vida / vidaController.VidaMax;
            vidaText.text = (porcentagem * 100).ToString("0") + "%";

            if (IsSemStun) return;

            if (stunHits >= maxStunHits) {
                stunHits = 0;
                semStunTimer = semStunTime;
            }
            else {
                stunHits++;
                stateMachine.SetState(hittedState);
            }
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


    void FixedUpdate()  {
        if (GameManager.instance.IsPaused()) return;
        stateMachine.Execute();

        if (semStunTimer > 0) semStunTimer -= Time.fixedDeltaTime;
    }

    public void OnAtaqueHit(GameObject other) {
        if (other != null && other.CompareTag("Player")) {
            other.GetComponent<PossuiVida>().LevarDano(ataque.dano);
        }
    }

    public AtacadorInfo GetInfo() {
        if (atacadorInfo != null) return atacadorInfo;
        
        atacadorInfo = new AtacadorInfo();
        atacadorInfo.animator = animator;
        atacadorInfo.attackHolder = attackHitboxHolder;
        atacadorInfo.attackTriggerName = "Attack";
        atacadorInfo.gameObject = gameObject;
        atacadorInfo.triggerMode = modoDeTriggerDeAnimacao;

        return atacadorInfo;
    }

    public void MoveWithAttack(float step, float progress) { /* Ainda não precisamos disso */ }


    // draw gizmos
    void OnDrawGizmosSelected() {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Vector3 tickness = new Vector3(1, 0.01f, 1);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, tickness);

        if (stateMachine == null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, rangeProcurando);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Vector3.zero, minRangeProximidade);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, rangePerderTarget);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, maxRangeProximidade);
        } else if (stateMachine.GetCurrentState() == idleState) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, rangeProcurando);
        } else if (stateMachine.GetCurrentState() == walkState) {
            Gizmos.matrix = Matrix4x4.TRS(target.transform.position, target.transform.rotation, tickness);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Vector3.zero, minRangeProximidade);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, rangePerderTarget);
        } else if (stateMachine.GetCurrentState() == attackState) {
            Gizmos.matrix = Matrix4x4.TRS(target.transform.position, target.transform.rotation, tickness);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, maxRangeProximidade);
        }
      
        Gizmos.matrix = oldMatrix;
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

    void OnDestroy() {
        if (Application.isPlaying) {
            OnDestroied?.Invoke();
        }
    }
}
