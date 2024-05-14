using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PossuiVida))]
public class Player : MonoBehaviour, IAtacador {
    public static Player instance;

    [Header("Atributos do Jogador")]
    public float calor;
    public float calorMax;
    public int dano = 1, defesa = 1, velocidade = 1;
    public int pecas = 0;
    public float moveSpeed = 3f;

    [Header("Inventario")]
    public Inventario inventario;
    [HideInInspector] public Arma arma;
    [HideInInspector] public Braco braco;
    public Transform mao, bracoHolder, pe;

    // State Machine
    public StateMachine stateMachine;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;


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
    }

    void Start() {

        vidaController.modificadorDeDano = (dano) => {
            return dano - defesa;
        };

        vidaController.onChange += (vida) => {
            UIController.HUD.UpdateVida(vida, vidaController.VidaMax);
        };

        vidaController.onDeath += () => {
            GameManager.instance.GameOver();
        };
        
        UpdateHUD();

        inventario = new Inventario();
        UIController.inventarioUI.SetupUI(inventario);
        UIController.inventarioUI.onSlotClick += HandleSlotClick;

        stateMachine = new StateMachine();
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);

        stateMachine.SetState(moveState);
    }

    void Update() {
        stateMachine.Execute();
        
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

    void HandleSlotClick(ItemData itemData) {
        if (itemData == null) return;

        TipoAbstrato especificacoes = itemData.prefab.GetComponent<TipoAbstrato>();
        if (!especificacoes.IsUsavel) return;

        if (inventario.RemoveItem(itemData)) {
            TipoAbstrato instancia = especificacoes;

            if (especificacoes.IsInstanciavel) {
                GameObject obj = itemData.CreateInstance();
                Item item = obj.GetComponent<Item>();
                instancia = item.GetTipo();
            }

            instancia.FazerAcao();
        }
    }

    public void UpdateHUD() {
        UIController.HUD.UpdateVida(vidaController.Vida, vidaController.VidaMax);
        UIController.HUD.UpdateCalor(calor, calorMax);
        UIController.HUD.UpdateAtributos(dano, defesa, velocidade);
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

    public void DiminuirCalor(int calor) {
        this.calor -= calor;
        if (this.calor < 0) this.calor = 0;

        UIController.HUD.UpdateCalor(this.calor, calorMax);
    }

    public void AumentarAtributo(int dano, int defesa, int velocidade) {
        this.dano += dano;
        this.defesa += defesa;
        this.velocidade += velocidade;

        UIController.HUD.UpdateAtributos(this.dano, this.defesa, this.velocidade);
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

    

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Item")) {
            // TEMPORARIO
            ItemDropado itemDropado = other.GetComponent<ItemDropado>();
            Item item = itemDropado.item;
            int quant = itemDropado.quantidade; // Lembrar de adicionar quantidade no inventário XD

            if (item != null) {
                if (inventario.AddItem(item.data))
                    Destroy(other.gameObject);
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
    }

    public void DesequiparArma() {
        if (arma == null) return;

        arma.onAttackEnd -= OnAttackEnded;
        inventario.AddItem(arma.GetComponent<Item>().data);
        Destroy(arma.gameObject);
        arma = null;

        if (stateMachine.GetCurrentState() == attackState) stateMachine.SetState(moveState);
    }

    public void EquiparBraco(Braco braco) {
        if (this.braco != null) DesequiparBraco();
        this.braco = braco;
        braco.transform.SetParent(bracoHolder);
        braco.transform.localPosition = Vector3.zero;
        braco.transform.localRotation = Quaternion.identity;
    }

    public void DesequiparBraco() {
        if (braco == null) return;

        inventario.AddItem(braco.GetComponent<Item>().data);
        Destroy(braco.gameObject);
        braco = null;
    }

    // GAMBIARRA FEIA
    void OnAttackEnded() {
        if (stateMachine.GetCurrentState() == attackState) {
            attackState.onAttackEnd();
        }
    }

    public Animator GetAnimator() { return animator; }

    public GameObject GetAttackHitboxHolder() { return meio; }
    public virtual void OnAtaqueHit(GameObject inimigo) {
        Debug.Log("Linha desnecessaria!");
    }
    public string AttackTriggerName() { return "Ataque"; }
}
