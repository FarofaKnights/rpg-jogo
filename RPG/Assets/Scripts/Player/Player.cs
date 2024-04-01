using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player instance;

    [Header("Atributos do Jogador")]
    public float vida;
    public float vidaMax;
    public float calor, calorMax;
    public int dano = 1, defesa = 1, velocidade = 1;
    public int pecas = 0;

    [Header("Inventario")]
    public Inventario inventario;
    [HideInInspector] public Arma arma;
    public Transform mao;


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        UpdateHUD();

        inventario = new Inventario();
        UIController.inventarioUI.SetupUI(inventario);
        UIController.inventarioUI.onSlotClick += HandleSlotClick;
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
        UIController.HUD.UpdateVida(vida, vidaMax);
        UIController.HUD.UpdateCalor(calor, calorMax);
        UIController.HUD.UpdateAtributos(dano, defesa, velocidade);
        UIController.HUD.UpdatePecas(pecas);
    }

    #region Stats
    public void TomarDano(int danoTomado) {
        int danoFinal = danoTomado - defesa;
        if (danoFinal < 0) danoFinal = 0;
        vida -= danoFinal;
        if (vida <= 0) {
            vida = 0;
            Debug.Log("Game Over");
        }

        UIController.HUD.UpdateVida(vida, vidaMax);
    }

    public void CurarVida(int cura) {
        vida += cura;
        if (vida > vidaMax) vida = vidaMax;

        UIController.HUD.UpdateVida(vida, vidaMax);
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
        UIController.HUD.UpdatePecas(pecas);
    }

    public void RemovePecas(int pecas) {
        this.pecas -= pecas;
        UIController.HUD.UpdatePecas(pecas);
    }

    

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
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
    }

    public void DesequiparArma() {
        if (arma == null) return;

        inventario.AddItem(arma.GetComponent<Item>().data);
        Destroy(arma.gameObject);
        arma = null;
    }
}
