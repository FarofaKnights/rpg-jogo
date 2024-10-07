using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [HideInInspector] public ItemData consumivelSelecionado;

    [Header("Informações")]
    public PlayerMovementInfo informacoesMovimentacao;
    public PlayerAimInfo informacoesMira;
    Vector3 velocity;

    // State Machine
    public StateMachine<IPlayerState> stateMachine;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;
    public PlayerAimState aimState;
    [HideInInspector] public bool canChangeStateThisFrame = true;

    [Header("Referências")]
    public Animator animator;
    CharacterController characterController;
    public Transform mao, bracoHolder, pe, saidaTiro;
    public GameObject meio;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera aimCam;
    public GameObject look, aimLook;
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
                cinemachineFreeLook.LookAt = look.transform;
                cinemachineFreeLook.Priority = 9;
            }

            Transform parent = thirdPersonCam.transform.parent;
            aimCam = parent.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
        }

        if (thirdPersonCam != null) {
            thirdPersonCam.Follow = transform;
            thirdPersonCam.Priority = 11;
        }

        if (aimCam != null) {
            aimCam.Follow = aimLook.transform;
            // aimCam.LookAt = aimLook.transform;
            aimCam.Priority = 9;
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


        GameManager.instance.controls.Player.Attack.performed += HandleAttackTriggered;
        GameManager.instance.controls.Player.Arm.performed += HandleArmTriggered;
        GameManager.instance.controls.Player.Interact.performed += HandleInteractTriggered;
    }

    public void HandleAttackTriggered(InputAction.CallbackContext ctx) {
        HandleAttackTriggered();
    }

    public void HandleAttackTriggered() {
        if (GameManager.instance.IsPaused()) return;
        if (stateMachine.GetCurrentState() == attackState || stateMachine.GetCurrentState() == aimState) return;

        if (arma != null) {
            stateMachine.SetState(attackState);
        }
    }

    public void HandleArmTriggered(InputAction.CallbackContext ctx) {
        HandleArmTriggered();
    }

    public void HandleArmTriggered() {
        if (GameManager.instance.IsPaused()) return;
        if (stateMachine.GetCurrentState() == attackState || stateMachine.GetCurrentState() == aimState) return;
        if (!canChangeStateThisFrame) return;

        Debug.Log("HandleArmTriggered: " + braco);

        if (braco.GetType() == typeof(BracoShooter)) {
            Debug.Log("entrou");
            stateMachine.SetState(aimState);
        } else if (braco != null) {
            Debug.Log("entrou2");
            braco.Ativar();
        }
    }

    public void HandleInteractTriggered(InputAction.CallbackContext ctx) {
        HandleInteractTriggered();
    }

    public void HandleInteractTriggered() {
        if (GameManager.instance.IsPaused()) return;
        if (stateMachine.GetCurrentState() == attackState || stateMachine.GetCurrentState() == aimState) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider hitCollider in hitColliders) {
            Drop drop = hitCollider.GetComponent<Drop>();
            if (drop != null) {
                drop.OnCollect();
            }
        }
    }

    void Update() {
        if (GameManager.instance.IsPaused()) return;
        
        stateMachine.GetCurrentState().Update();
        canChangeStateThisFrame = true;
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

    void OnDestroy() {
        if (GameManager.instance != null) {
            GameManager.instance.controls.Player.Attack.performed -= HandleAttackTriggered;
            GameManager.instance.controls.Player.Arm.performed -= HandleArmTriggered;
            GameManager.instance.controls.Player.Interact.performed -= HandleInteractTriggered;
        }
        
    }
}
