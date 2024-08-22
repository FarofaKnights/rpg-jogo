using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Defective.JSON;

[RequireComponent(typeof(PossuiVida))]
public class Player : MonoBehaviour, Saveable {

    // Singleton
    public static Player instance;
    public static StatsController Stats { get { return instance.stats; } }
    public static AtributosController Atributos { get { return instance.atributos; } }
    public static InventarioManager Inventario { get { return instance.inventario; } }


    [Header("Atributos do Jogador")]
    public StatsController stats;
    public AtributosController atributos;

    [Header("Inventario")]
    public InventarioManager inventario;
    public ItemData[] itensJaPossuidos;
    [HideInInspector] public Arma arma;
    [HideInInspector] public Braco braco;

    [Header("Movimentacao")]
    public PlayerMovementInfo informacoesMovimentacao;
    Vector3 velocity;

    // State Machine
    public StateMachine<IPlayerState> stateMachine;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;
    public PlayerAimState aimState;

    [Header("Referências")]
    public Animator animator;
    CharacterController characterController;
    public Transform mao, bracoHolder, pe, saidaTiro;
    public GameObject meio;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera aimCam;
    PossuiVida vidaController;

    void Awake() {
        // Singleton
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        vidaController = GetComponent<PossuiVida>();
        characterController = GetComponent<CharacterController>();

        // Setup cameras (pois ninguém vai fazer por conta própria e ainda vai falar que "tá dando erro")
        CinemachineFreeLook cinemachineFreeLook = GameObject.FindObjectOfType<CinemachineFreeLook>();

        if (cinemachineFreeLook != null) {
            thirdPersonCam = cinemachineFreeLook.GetComponent<CinemachineFreeLook>();

            if (cinemachineFreeLook != null) {
                cinemachineFreeLook.Follow = transform;
                cinemachineFreeLook.LookAt = transform.Find("Look");
            }

            Transform parent = thirdPersonCam.transform.parent;
            aimCam = parent.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
        }

        if (thirdPersonCam != null) {
            thirdPersonCam.Follow = transform;
        }

        if (aimCam != null) {
            aimCam.Follow = transform.Find("Look");
            aimCam.LookAt = transform.Find("Look");
            aimCam.gameObject.SetActive(false);
        }

        
    }

    void Start() {
        // Inicializa InventarioManager
        inventario = new InventarioManager();

        // Inicializa AtributosController
        if (atributos == null) atributos = new AtributosController();
        atributos.Initialize();

        // Inicializa StatsController
        if (stats == null) stats = new StatsController(1, 1, 1, 1);
        stats.TriggerChange();
        
        // Inicializa StateMachine
        stateMachine = new StateMachine<IPlayerState>();
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);
        aimState = new PlayerAimState(this);
        stateMachine.SetState(moveState);

        // Define que ao morrer chama o GameOver
        vidaController.onDeath += () => { GameManager.instance.GameOver(); };

        // Define chamada de evento do stats
        stats.OnChange += (string a, int b) => AplicarStats();
        AplicarStats();

        // Seta itens que o player começa com (não usamos isso, mas existe)
        if (itensJaPossuidos != null) {
            foreach (ItemData item in itensJaPossuidos) {
                if (!inventario.ContainsItem(item)) {
                    inventario.AddItem(item);
                    if ((item.tipo == ItemData.Tipo.Arma && arma == null) || (item.tipo == ItemData.Tipo.Braco && braco == null)) {
                        inventario.HandleSlotClick(item);
                    }
                }
            }
        }
    }

    void Update() {
        // Não queremos interações extras enquanto o player está atacando (cliques de combo são tratados no próprio estado)
        if (stateMachine.GetCurrentState() == attackState) return;

        if (Input.GetMouseButtonDown(0)) {
            if (arma != null) {
                stateMachine.SetState(attackState);
            }
        }

        if (Input.GetMouseButton(1)) {
            Debug.Log("Ataque especial: " + braco);
            if (braco != null) {
                stateMachine.SetState(aimState);
                braco.Ativar();
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (braco != null) {
                stateMachine.SetState(aimState);
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            if (braco != null) {
                stateMachine.SetState(moveState);
                braco.Ativar();
            }
        }
    }

    void FixedUpdate() {
        if (GameManager.instance.IsPaused()) return;
        stateMachine.Execute();

        // Movement
        bool isGrounded = IsGrounded();

        if (isGrounded) {
            velocity.y = 0f;
        } else {
            velocity.y = informacoesMovimentacao.gravity;
        }

        characterController.Move(velocity * Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Atualiza os atributos baseado no valor dos Stats
    public void AplicarStats() {
        IAtributo<float> vida = atributos.vida;
        float adicionalVida = stats.GetAdicionalVida(vida.GetMaxBase());
        vida.SetMax(vida.GetMaxBase() + adicionalVida);
        if (vida.Get() < vida.GetMax()) vida.Add(adicionalVida);

        
        IAtributo<float> calor = atributos.calor;
        float adicionalCalor = stats.GetAdicionalCalor(calor.GetMaxBase());
        calor.SetMax(calor.GetMaxBase() + adicionalCalor);
        if (calor.Get() < calor.GetMax()) calor.Add(adicionalCalor);
    }

    
    // Coletar item ao passar por cima
    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Drop>() != null) {
            Drop drop = other.GetComponent<Drop>();
            drop.OnCollect();
        }
    }

    // Teleporta player para uma posição (pois o CharacterController sobreescreve o transform)
    public void TeleportTo(Vector3 position) {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
    }

    #region Saveable implementation

    public JSONObject Save() {
        JSONObject obj = new JSONObject();
        
        obj.AddField("destreza", stats.destreza);
        obj.AddField("forca", stats.forca);
        obj.AddField("vida", stats.vida);
        obj.AddField("calor", stats.calor);

        int pecas = atributos.pecas.Get();
        obj.AddField("pecas", pecas);

        if (arma != null) {
            ItemData armaData = arma.GetComponent<Item>().data;
            obj.AddField("arma", armaData.ToSaveString());
        }

        if (braco != null) {
            ItemData bracoData = braco.GetComponent<Item>().data;
            obj.AddField("braco", bracoData.ToSaveString());
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        stats.SetDestreza(obj.GetField("destreza").intValue);
        stats.SetForca(obj.GetField("forca").intValue);
        stats.SetVida(obj.GetField("vida").intValue);
        stats.SetCalor(obj.GetField("calor").intValue);

        int pecas = obj.GetField("pecas").intValue;
        atributos.pecas.Set(pecas);
    }

    // Load de equipamentos ocorre separadamente, após o load do inventário
    public void LoadEquipados(JSONObject obj) {
        if (obj.HasField("arma")) {
            string armaString = obj.GetField("arma").stringValue;
            ItemData armaData = ItemManager.instance.GetItemData(armaString);
            inventario.HandleSlotClick(armaData);
        }

        if (obj.HasField("braco")) {
            string bracoString = obj.GetField("braco").stringValue;
            ItemData bracoData = ItemManager.instance.GetItemData(bracoString);
            inventario.HandleSlotClick(bracoData);
        }
    }

    #endregion

    // O Ataque pode ocorrer de mexer o player (isso acontece), e aqui nós tratamos o movimento
    public void MoveWithAttack(float step, float progress) {
        Vector3 move = transform.forward * step;
        characterController.Move(move);
    }

    public bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, informacoesMovimentacao.groundLayer);
    }

    public void SetMovement(Vector3 move) {
        velocity = move;
    }
}
