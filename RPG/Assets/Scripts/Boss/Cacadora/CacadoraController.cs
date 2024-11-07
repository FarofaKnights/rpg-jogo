using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacadoraController : MonoBehaviour, IAtacador {
    [HideInInspector] public IChainedState[] sequencia;
    public StateMachine<IChainedState> stateMachine;

    // Ataque
    [Header("Ataque")]
    public AtaqueInfo ataque, investida;
    [SerializeField] GameObject attackHitboxHolder;
    AtacadorInfo atacadorInfo;
    public TriggerMode modoDeTriggerDeAnimacao = TriggerMode.Trigger;


    // Referencias
    [HideInInspector] public CacadoraArena arena;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;
    PossuiVida vidaController;

    // Valores
    [Header("Valores")]
    public float speedOutside = 5f;
    public float speedNormal = 3.5f;
    public float speedInvestida = 10f;
    public float playerAttackRange = 1.5f;


    bool comecou = false;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        vidaController = GetComponent<PossuiVida>();


        stateMachine = new StateMachine<IChainedState>();
    }


    void Start() {
        arena = FindObjectOfType<CacadoraArena>();

        vidaController.onChange += (vida) => {
            UIController.HUD.UpdateBossVida(vidaController.Vida, vidaController.VidaMax);
        };

        vidaController.onDeath += () => {
            UIController.HUD.ShowBossVida(false);
        };
    }

    // Chamada por message na missão
    public void Comecar() {
        comecou = true;
        GerarNovaSequencia();

        UIController.HUD.ShowBossVida(true);
        UIController.HUD.UpdateBossVida(vidaController.Vida, vidaController.VidaMax);
    }

    void FixedUpdate() {
        if (!comecou) return;
        stateMachine.Execute();
    }

    public void GerarNovaSequencia() {
        // Sequencia dash n ta funfando
        sequencia = Random.Range(0, 2) == 0 ? SequenciaBusca() : SequenciaDash();
        sequencia = SequenciaBusca();

        agent.speed = speedNormal;
        
        stateMachine.SetState(sequencia[0]);
    }

    public IChainedState[] SequenciaDeAndar() {
        IChainedState[] estados = new IChainedState[4];

        DualPosition posInicial = arena.GetRandomInOutPosition();
        DualPosition posFinal = arena.GetRandomInOutPosition();

        estados[0] = new TeleportState(stateMachine, agent, posInicial.pos2);
        estados[0].OnEnd = () => agent.speed = speedOutside;
        estados[1] = new AndarState(stateMachine, animator, agent, posInicial.pos1);
        estados[1].OnEnd = () => agent.speed = speedInvestida;
        estados[2] = new AndarState(stateMachine, animator, agent, posFinal.pos1);
        estados[3] = new AndarState(stateMachine, animator, agent, posFinal.pos2);

        for (int i = 0; i < estados.Length - 1; i++) {
            estados[i].nextState = estados[i + 1];
        }

        estados[3].OnEnd = () => {
            GerarNovaSequencia();
        };

        return estados;
    }

    public IChainedState[] SequenciaBusca() {
        IChainedState[] estados = new IChainedState[2];

        estados[0] = new SearchState(stateMachine, animator, agent, Player.instance.transform, playerAttackRange);
        estados[1] = new AtaqueState(stateMachine, this, ataque);

        estados[0].nextState = estados[1];

        estados[1].OnEnd = () => {
            GerarNovaSequencia();
        };

        return estados;
    }

    public IChainedState[] SequenciaDash() {
        IChainedState[] estados = new IChainedState[2];

        estados[0] = new SearchState(stateMachine, animator, agent, Player.instance.transform, playerAttackRange);
        estados[1] = new AtaqueState(stateMachine, this, investida);

        estados[0].nextState = estados[1];

        estados[1].OnEnd = () => {
            GerarNovaSequencia();
        };

        return estados;
    }


    // Ataque 
    public bool OnAtaqueHit(GameObject other) {
        if (other != null && other.CompareTag("Player")) {
            other.GetComponent<PossuiVida>().LevarDano(ataque.dano);
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

    public void MoveWithAttack(float step, float progress) {/*
        Vector3 move = transform.forward * step;
        controller.Move(move);*/
    }
}
