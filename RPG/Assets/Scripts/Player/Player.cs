using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[RequireComponent(typeof(PossuiVida))]
public class Player : MonoBehaviour, IAtacador, Saveable {
    public static Player instance;
    public static StatsController Stats { get { return instance.stats; } }
    public static AtributosController Atributos { get { return instance.atributos; } }



    [Header("Atributos do Jogador")]
    public StatsController stats;
    public AtributosController atributos;
    public float moveSpeed = 3f;

    [Header("Inventario")]
    public InventarioManager inventario;
    public ItemData[] itensJaPossuidos;
    [HideInInspector] public Arma arma;
    [HideInInspector] public Braco braco;
    public Transform mao, bracoHolder, pe, saidaTiro;

    // State Machine
    public StateMachine<IPlayerState> stateMachine;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;

    [Header("Referências")]
    public Camera cam;
    public float cameraSpeed = 10f;

    PossuiVida vidaController;
    public Animator animator;
    public GameObject meio;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        vidaController = GetComponent<PossuiVida>();
        if (cam == null) cam = Camera.main;

        inventario = new InventarioManager();

        if (atributos == null) atributos = new AtributosController();
        atributos.Initialize();

        if (stats == null) stats = new StatsController(1, 1, 1, 1);
        else {
            stats.SetDestreza(1);
            stats.SetForca(1);
            stats.SetVida(1);
            stats.SetCalor(1);
        }
    }

    void Start() {
        vidaController.onChange += (vida) => {
            UIController.HUD.UpdateVida(vida, vidaController.VidaMax);
        };

        vidaController.onDeath += () => {
            GameManager.instance.GameOver();
        };
        
        

        stateMachine = new StateMachine<IPlayerState>();
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);

        stateMachine.SetState(moveState);

        stats.OnChange += (string a, int b) => AplicarStats();
        AplicarStats();

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

        UpdateHUD();
    }

    void Update() {
        Debug.DrawLine(Vector3.zero, transform.position, Color.red);

        if (stateMachine.GetCurrentState() == attackState) return;

        if (Input.GetMouseButtonDown(0)) {
            if (arma != null) {
                stateMachine.SetState(attackState);
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("Ataque especial: " + braco);
            if (braco != null) {
                braco.Ativar();
            }
        }
    }

    void FixedUpdate() {
        if (GameManager.instance.IsPaused()) return;
        stateMachine.Execute();
    }

    public void UpdateHUD() {
        UIController.HUD.UpdateVida(vidaController.Vida, vidaController.VidaMax);
        UIController.HUD.SetArmaEquipada(arma);
    }

    public void AplicarStats() {
        IAtributo<float> vida = atributos.vida;
        IAtributo<float> calor = atributos.calor;
        Debug.Log(vida);
        float adicionalVida = stats.GetAdicionalVida(vida.GetMax());

        vida.AddMax(adicionalVida);
        if (vida.Get() < vida.GetMax()) vida.Add(adicionalVida);


        float adicionalCalor = stats.GetAdicionalCalor(calor.GetMax());

        calor.AddMax(adicionalCalor);
        if (calor.Get() < calor.GetMax()) calor.Add(adicionalCalor);
    }

    
    #region Itens e Equipamentos
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Item")) {
            // TEMPORARIO
            ItemDropado itemDropado = other.GetComponent<ItemDropado>();
            Item item = itemDropado.item;
            int quant = itemDropado.quantidade; // Lembrar de adicionar quantidade no inventário XD

            if (item != null) {
                if (inventario.AddItem(item.data)) {
                    Destroy(other.gameObject);

                    // Se tiver vazio, equipa arma ou braço
                    if ((item.data.tipo == ItemData.Tipo.Arma && arma == null) || (item.data.tipo == ItemData.Tipo.Braco && braco == null)) {
                        inventario.HandleSlotClick(item.data);
                    }
                }          
            }
        } else if (other.GetComponent<PecaDropado>() != null) {
            PecaDropado peca = other.GetComponent<PecaDropado>();
            int quant = peca.quantidade;
            atributos.pecas.Add(quant);
            Destroy(other.gameObject);
        }
    }

    public void EquiparArma(Arma arma) {
        if (this.arma != null) DesequiparArma();
        this.arma = arma;
        arma.transform.SetParent(mao);
        arma.transform.localPosition = Vector3.zero;
        arma.transform.localRotation = Quaternion.identity;
        arma.onAttackEnd += OnAttackEnded;

        UIController.equipamentos.RefreshUI();
        UIController.HUD.SetArmaEquipada(arma);
    }

    public void DesequiparArma() {
        if (arma == null) return;

        arma.onAttackEnd -= OnAttackEnded;
        Destroy(arma.gameObject);
        arma = null;
        UIController.equipamentos.RefreshUI();
        UIController.HUD.SetArmaEquipada(null);

        if (stateMachine.GetCurrentState() == attackState) {
            stateMachine.SetState(moveState);
        }
    }

    public void EquiparBraco(Braco braco) {
        if (this.braco != null) DesequiparBraco();
        this.braco = braco;
        braco.transform.SetParent(bracoHolder);
        braco.transform.localPosition = Vector3.zero;
        braco.transform.localRotation = Quaternion.identity;

        UIController.equipamentos.RefreshUI();
    }

    public void DesequiparBraco() {
        if (braco == null) return;

        Destroy(braco.gameObject);
        braco = null;

        UIController.equipamentos.RefreshUI();
    }

    #endregion

    void OnAttackEnded() {}

    public Animator GetAnimator() { return animator; }

    public GameObject GetAttackHolder() { return meio; }
    public virtual void OnAtaqueHit(GameObject inimigo) { }
    public string AttackTriggerName() { return "Ataque"; }
    public GameObject GetSelf() { return gameObject; }
    public TriggerMode GetTriggerMode() { return TriggerMode.Trigger; }



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

        // Salvar inventário, arma e braço

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

    public void TeleportTo(Vector3 position) {
        // Character Controller won't let you teleport the player
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }
}
