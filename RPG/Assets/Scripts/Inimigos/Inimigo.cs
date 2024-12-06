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
    public float empurradoAoSofrerHit = 0.1f;
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

    [Header("Animação de morte")]
    public bool possuiAnimacaoMorte = false;
    public float tempoAteDissolver = 1f;
    public float tempoDissolvendo = 1f;


    [Header("MISC")]
    public GameObject target;
    public int recompensaPecas = 10;
    PossuiVida vidaController;
    public GameObject getHitParticles;
    public AudioSource AlertSound;
    public AudioSource attackSound;
    public Vector2 variacaoTom = new Vector2(1f,1f);
    [HideInInspector] public bool podeSeguir = true;
    public bool interromperSom = true;
    public bool detectado = false;
    public System.Action OnDestroied; // Diferente do OnDeath, esse é chamado quando o inimigo é destruído, isso tbm conta destruição por integridade (foi morto em outro save e é destruido para não ser carregado)


    [Header("Configurações de Ataque")]
    public AtaqueInfo ataque;
    public enum ComoAtaca { AtaqueUnico, AtaquesAleatorios, AtaquesEmSequencia }
    protected int ataqueAtual = -1; // Utilizado apenas em AtaquesEmSequencia
    public ComoAtaca comoAtaca = ComoAtaca.AtaqueUnico;
    public AtaqueInfo[] ataques;
    [SerializeField] GameObject attackHitboxHolder;
    AtacadorInfo atacadorInfo;

    [Header("Patrulhando")]
    public bool patrulha = false;
    public List<Transform> pontosPatrulha = new List<Transform>();
    public float tempoEsperaPatrulha = 2f;
    public float velocidadePatrulha = 1f;
    public float distanciaMinimaPatrulha = 0.1f;

    public enum TipoPatrulha { Ciclica, PingPong, Aleatoria }
    public TipoPatrulha tipoPatrulha = TipoPatrulha.Ciclica;

    public StateMachine<IEnemyState> stateMachine;
    public EnemyIdleState idleState;
    public EnemyAttackState attackState;
    public EnemyWalkState walkState;
    public EnemyHittedState hittedState;
    public EnemyDeathState deathState;
    public EnemyPatrolState patrolState;

    [HideInInspector] public int hittedDir = 0;
    [HideInInspector] public int deathID = 0;

    
    string estado_atual = "nenhum";
    

    protected virtual void Awake() {
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

                if (vida > 0 || !possuiAnimacaoMorte)
                    stateMachine.SetState(hittedState);
            }
        };

        vidaController.onDeath += (DamageInfo danoInfo) => {
            if (danoInfo.origem == Player.instance.gameObject)
                Player.Atributos.pecas.Add(recompensaPecas);

            Morrer();
        };

        vidaController.SetDestroyOnDeath(false);
    }

    protected virtual void Start() {
        stateMachine = new StateMachine<IEnemyState>();
        idleState = new EnemyIdleState(this);
        attackState = new EnemyAttackState(this);
        walkState = new EnemyWalkState(this);
        hittedState = new EnemyHittedState(this);
        deathState = new EnemyDeathState(this);
        patrolState = new EnemyPatrolState(this);

        stateMachine.OnStateChange += (state) => {
            estado_atual = state.ToString();
        };

        if (patrulha) stateMachine.SetState(patrolState);
        else stateMachine.SetState(idleState);
    }


    void FixedUpdate()  {
        if (GameManager.instance.IsPaused()) return;
        stateMachine.Execute();

        if (semStunTimer > 0) semStunTimer -= Time.fixedDeltaTime;
    }

    public bool OnAtaqueHit(GameObject other) {
        if (other != null && other.CompareTag("Player")) {
            other.GetComponent<PossuiVida>().LevarDano(GetDano());
            return true;
        }
        return false;
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

    public void MoveWithAttack(float step, float progress) {
        Vector3 move = transform.forward * step;
        controller.Move(move);
    }

    public virtual AtaqueInstance GetAtaque(out float waitBeforeLeaving) {
        waitBeforeLeaving = 0;

        if (comoAtaca == ComoAtaca.AtaqueUnico) return ataque.Atacar(this);
        else if (comoAtaca == ComoAtaca.AtaquesAleatorios) {
            ataqueAtual = Random.Range(0, ataques.Length);
            return ataques[ataqueAtual].Atacar(this);
        }
        else if (comoAtaca == ComoAtaca.AtaquesEmSequencia) {
            ataqueAtual++;
            if (ataqueAtual >= ataques.Length) ataqueAtual = 0;
            return ataques[ataqueAtual].Atacar(this);
        }
        return ataque?.Atacar(this);
    }

    public virtual DamageInfo GetDano() {
        if (comoAtaca == ComoAtaca.AtaqueUnico) return ataque.dano;
        else if (comoAtaca == ComoAtaca.AtaquesAleatorios) return ataques[ataqueAtual].dano;
        else if (comoAtaca == ComoAtaca.AtaquesEmSequencia) return ataques[ataqueAtual].dano;
        return ataque.dano;
    }

    public void CheckForPlayer() {
        if (!podeSeguir) return;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangeProcurando);
        foreach (Collider collider in colliders) {
            float yDist = Mathf.Abs(collider.transform.position.y - transform.position.y);
            if (yDist > maxYProcurando) continue;

            if (collider.CompareTag("Player")) {
                target = collider.gameObject;
                stateMachine.SetState(walkState);
                return;
            }
        }
    }

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


        if (patrulha) {
            Gizmos.color = Color.green;
            for (int i = 0; i < pontosPatrulha.Count; i++) {
                Gizmos.DrawWireSphere(pontosPatrulha[i].position, 0.25f);
                if (tipoPatrulha != TipoPatrulha.Aleatoria && i < pontosPatrulha.Count - 1) {
                    Gizmos.DrawLine(pontosPatrulha[i].position, pontosPatrulha[i + 1].position);
                }
            }

            if (tipoPatrulha == TipoPatrulha.Ciclica) {
                Gizmos.DrawLine(pontosPatrulha[pontosPatrulha.Count - 1].position, pontosPatrulha[0].position);
            }
        }
    }

    public void SpawnParticle() {
        if (!getHitParticles) {
            Debug.LogWarning("Não foi definido um prefab de partículas para o inimigo " + gameObject.name);
            return;
        }

        GameObject particle = Instantiate(getHitParticles, attackHitboxHolder.transform.position, transform.rotation);
        particle.transform.SetParent(this.gameObject.transform);
    }

    public void Morrer() {
        if (stateMachine.GetCurrentState() == deathState) return;
        stateMachine.SetState(deathState);
    }

    void OnDestroy() {
        if (Application.isPlaying) {
            OnDestroied?.Invoke();
        }
    }
    
    public void PodeSeguir(bool pode) {
        if (!pode) {
            if (stateMachine == null) { // Isso não faz o menor sentido, ainda sim, alguns inimigos caem nessa situação ????
                return;
            }
            target = null;
            if (patrulha) stateMachine.SetState(patrolState);
            else stateMachine.SetState(idleState);
        }

        podeSeguir = pode;
    }

    public void SetTarget(GameObject target) {
        this.target = target;
        if (stateMachine == null) return;
        stateMachine.SetState(walkState);
    }
}
