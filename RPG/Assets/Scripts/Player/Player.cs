using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[RequireComponent(typeof(PossuiVida))]
public class Player : MonoBehaviour, IAtacador, Saveable {
    public static Player instance;

    [Header("Atributos do Jogador")]
    public float calor;
    public float calorMax;
    public int statDestreza, statForca, statVida, statCalor;
    public int pecas = 0;
    public float moveSpeed = 3f;

    [Header("Inventario")]
    public InventarioManager inventario;
    [HideInInspector] public Arma arma;
    [HideInInspector] public Braco braco;
    public Transform mao, bracoHolder, pe;

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
    }

    void Start() {
        /*
        vidaController.modificadorDeDano = (dano) => {
            return dano - defesa;
        };
        */

        vidaController.onChange += (vida) => {
            UIController.HUD.UpdateVida(vida, vidaController.VidaMax);
        };

        vidaController.onDeath += () => {
            GameManager.instance.GameOver();
        };
        
        UpdateHUD();

        stateMachine = new StateMachine<IPlayerState>();
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);

        stateMachine.SetState(moveState);
    }

    void Update() {
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
        UIController.HUD.UpdateCalor(calor, calorMax);
        UIController.HUD.UpdateAtributos(statDestreza, statForca, statVida, statCalor);
        UIController.HUD.UpdatePecas(pecas);
    }

    #region Stats
    public void TomarDano(int danoTomado) {
        GetComponent<PossuiVida>().LevarDano(danoTomado);
    }

    public void CurarVida(int cura) {
        GetComponent<PossuiVida>().Curar(cura);
    }

    public void AumentarCalor(int calor) {
        this.calor += calor;
        if (this.calor > calorMax) this.calor = calorMax;

        UIController.HUD.UpdateCalor(this.calor, calorMax);
    }

    public void SobreescreverCalor(int calor) {
        this.calor = calor;
        UIController.HUD.UpdateCalor(this.calor, calorMax);
    }

    public void DiminuirCalor(int calor) {
        this.calor -= calor;
        if (this.calor < 0) this.calor = 0;

        UIController.HUD.UpdateCalor(this.calor, calorMax);
    }

    public void AumentarAtributo(int destreza, int forca, int vida, int calor) {
        this.statDestreza += destreza;
        this.statForca += forca;
        this.statVida += vida;
        this.statCalor += calor;

        UIController.HUD.UpdateAtributos(this.statDestreza, this.statForca, this.statVida, this.statCalor);
    }

    #endregion

    public void AddPecas(int pecas) {
        this.pecas += pecas;
        UIController.HUD.UpdatePecas(this.pecas);
    }

    public void RemovePecas(int pecas) {
        this.pecas -= pecas;
        UIController.HUD.UpdatePecas(pecas);
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
    }

    public void DesequiparArma() {
        if (arma == null) return;

        arma.onAttackEnd -= OnAttackEnded;
        Destroy(arma.gameObject);
        arma = null;
        UIController.equipamentos.RefreshUI();

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

    public GameObject GetAttackHitboxHolder() { return meio; }
    public virtual void OnAtaqueHit(GameObject inimigo) {
        Debug.Log("Linha desnecessaria!");
    }
    public string AttackTriggerName() { return "Ataque"; }


    public JSONObject Save() {
        JSONObject obj = new JSONObject();
        
        obj.AddField("destreza", statDestreza);
        obj.AddField("forca", statForca);
        obj.AddField("vida", statVida);
        obj.AddField("calor", statCalor);

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
        statDestreza = obj.GetField("destreza").intValue;
        statForca = obj.GetField("forca").intValue;
        statVida = obj.GetField("vida").intValue;
        statCalor = obj.GetField("calor").intValue;
        UIController.HUD.UpdateAtributos(statDestreza, statForca, statVida, statCalor);

        pecas = obj.GetField("pecas").intValue;
        UIController.HUD.UpdatePecas(pecas);
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
