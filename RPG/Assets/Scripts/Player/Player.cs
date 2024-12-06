using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Defective.JSON;

[RequireComponent(typeof(PossuiVida))]
public class Player : MonoBehaviour, Saveable, IEquipador, Sentidor {

    // Singleton refs
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
    public float stunTime = 0.5f;
    public Vector3 velocity;
    AtacadorInfo atacadorInfo;


    // State Machine
    public StateMachine<IPlayerState> stateMachine;
    public PlayerMoveState moveState;
    public PlayerDashState dashState;
    public PlayerAttackState attackState;
    public PlayerAimState aimState;
    public PlayerHittedState hittedState;
    [HideInInspector] public bool canChangeStateThisFrame = true;


    [Header("Referências")]
    public Animator animator;
    CharacterController characterController;
    public Transform mao, bracoHolder, pe, saidaTiro;
    public GameObject meio;
    public CinemachineFreeLook thirdPersonCam;
    public CinemachineVirtualCamera aimCam;
    public CinemachineVirtualCamera lookDirectionAuxCam;
    public GameObject look, aimLook;
    public Transform defaultBraco;
    public Transform bracoHolderBase, bracoHolderMeio, bracoHolderMao;
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

        // Setup cameras
        SetupCameras();

        // Inicializa InventarioManager
        inventario = new InventarioManager();
        if (atributos == null) atributos = new AtributosController();
        if (stats == null) stats = new StatsController(1, 1, 1, 1);
        
        
        // Inicializa StateMachine
        stateMachine = new StateMachine<IPlayerState>();
        moveState = new PlayerMoveState(this);
        dashState = new PlayerDashState(this);
        attackState = new PlayerAttackState(this);
        aimState = new PlayerAimState(this);
        hittedState = new PlayerHittedState(this);
        stateMachine.SetState(moveState);
    }

    void SetupCameras() {
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
        atributos.Initialize();
        stats.TriggerChange();

        // Define que ao morrer chama o GameOver
        vidaController.onDeath += (DamageInfo danoInfo) => { GameManager.instance.GameOver(); };
        vidaController.onDamage += OnBeingHit;

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

    public AtacadorInfo GetInfo() {
        if (atacadorInfo != null) return atacadorInfo;

        atacadorInfo = new AtacadorInfo();
        atacadorInfo.animator = animator;
        atacadorInfo.attackHolder = meio;
        atacadorInfo.attackTriggerName = "Ataque";
        atacadorInfo.gameObject = gameObject;
        atacadorInfo.triggerMode = TriggerMode.Trigger;

        return atacadorInfo;
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
        if (braco == null) return;

        if (braco.GetType() == typeof(BracoShooter)) {
            stateMachine.SetState(aimState);
        } else {
            braco.Ativar();
        }
    }

    public void HandleInteractTriggered(InputAction.CallbackContext ctx) {
        HandleInteractTriggered();
    }

    public void HandleInteractTriggered() {
        if (GameManager.instance.IsPaused()) return;
        if (stateMachine.GetCurrentState() == attackState || stateMachine.GetCurrentState() == aimState) return;

        List<Drop> dropsPegos = new List<Drop>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider hitCollider in hitColliders) {
            Drop drop = hitCollider.GetComponent<Drop>();
            if (drop != null && !dropsPegos.Contains(drop)) {
                drop.OnCollect();
                dropsPegos.Add(drop);
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
        velocity.y = isGrounded ? 0f : informacoesMovimentacao.gravity;

        characterController.Move(velocity * Time.fixedDeltaTime);
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

    public void Equipar(Equipamento equipamento) {
        System.Type type = equipamento.GetType();

        if (type == typeof(Arma)) {
            inventario.EquiparArma((Arma) equipamento);
        } else if (typeof(Braco).IsAssignableFrom(type)) {
            inventario.EquiparBraco((Braco) equipamento);
        }
    }

    public void Desequipar(Equipamento equipamento) {
        System.Type type = equipamento.GetType();

        if (type == typeof(Arma)) {
            inventario.DesequiparArma();
        } else if (typeof(Braco).IsAssignableFrom(type)) {
            inventario.DesequiparBraco();
        }
    }

    // O Ataque pode ocorrer de mexer o player (isso acontece), e aqui nós tratamos o movimento
    public void MoveWithAttack(float step, float progress) {
        Vector3 move = transform.forward * step;
        characterController.Move(move);
    }

    public bool OnAtaqueHit(GameObject inimigo) {
        if (!inimigo.CompareTag("Inimigo")) return false;

        AtaqueInfo ataque = arma.ataque;
        int ataqueIndex = arma.ataqueIndex;

        atributos.calor.Add(10);
        float adicional = stats.GetAdicionalForca(ataque.dano.dano);
        ataque.dano.danoAdicional += adicional;

        // Informa a direção do ataque ao inimigo (para animações de hit específicas)
        Inimigo inimigoScript = inimigo.GetComponent<Inimigo>();
        if (inimigoScript != null) {
            inimigoScript.hittedDir = ataqueIndex;
            inimigoScript.deathID = arma.GetTipoID();
        }
        
        PossuiVida vidaInimigo = inimigo.GetComponent<PossuiVida>();
        if (inimigo.GetComponent<PossuiVida>() == null)
            vidaInimigo = inimigo.GetComponentInChildren<PossuiVida>();
        
        if (vidaInimigo != null)
            vidaInimigo.LevarDano(ataque.dano);
        
        return true;
    }

    public void OnBeingHit(DamageInfo dano) {
        if (stateMachine.GetCurrentState() == hittedState) return;
        if (dano.formaDeDano == FormaDeDano.Passivo) return;
        stateMachine.SetState(hittedState);
    }

    public void SentirTemperatura(float modTemperatura) {
        atributos.calor.Add(modTemperatura);
    }

    public bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, informacoesMovimentacao.groundLayer);
    }

    public void SetMovement(Vector3 move) {
        velocity = move;
    }

    public void LookForward() {
        StartCoroutine(LookForwardCoroutine());
    }

    public IEnumerator LookForwardCoroutine() {
        lookDirectionAuxCam.Priority = 100;
        thirdPersonCam.transform.position = lookDirectionAuxCam.transform.position;
        thirdPersonCam.transform.rotation = lookDirectionAuxCam.transform.rotation;
        lookDirectionAuxCam.gameObject.SetActive(true);

        yield return null;

        lookDirectionAuxCam.gameObject.SetActive(false);
        lookDirectionAuxCam.Priority = 0;
    }

    public void RotatePlayerToCamera() {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        
        characterController.enabled = false;
        transform.rotation = Quaternion.LookRotation(camForward);
        characterController.enabled = true;
    }

    public void SetarControle(bool controle) {
        enabled = controle;

        if (!controle)
            animator.SetTrigger("Cancel");
    }

    void OnDestroy() {
        if (GameManager.instance != null) {
            GameManager.instance.controls.Player.Attack.performed -= HandleAttackTriggered;
            GameManager.instance.controls.Player.Arm.performed -= HandleArmTriggered;
            GameManager.instance.controls.Player.Interact.performed -= HandleInteractTriggered;
        }
        
    }
}
