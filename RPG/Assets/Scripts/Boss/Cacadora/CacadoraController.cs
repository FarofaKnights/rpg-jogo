using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IChainedState: IState {
    IChainedState nextState { get; set; }
    System.Action OnEnd { get; set; }
}

public class CacadoraController : MonoBehaviour {
    [HideInInspector] public IChainedState[] sequencia;

    // Referencias
    [HideInInspector] public CacadoraArena arena;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;

    public StateMachine<IChainedState> stateMachine;

    // Valores
    public float speedOutside = 5f;
    public float speedInside = 10f;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        stateMachine = new StateMachine<IChainedState>();
    }


    void Start() {
        arena = FindObjectOfType<CacadoraArena>();

        GerarNovaSequencia();
    }

    // Update is called once per frame
    void FixedUpdate() {
        stateMachine.Execute();
    }

    public void GerarNovaSequencia() {
        sequencia = SequenciaDeAndar();
        stateMachine.SetState(sequencia[0]);
    }

    public IChainedState[] SequenciaDeAndar() {
        IChainedState[] estados = new IChainedState[4];

        DualPosition posInicial = arena.GetRandomInOutPosition();
        DualPosition posFinal = arena.GetRandomInOutPosition();

        estados[0] = new TeleportState(stateMachine, agent, posInicial.pos2);
        estados[0].OnEnd = () => agent.speed = speedOutside;
        estados[1] = new AndarState(stateMachine, animator, agent, posInicial.pos1);
        estados[1].OnEnd = () => agent.speed = speedInside;
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
}
