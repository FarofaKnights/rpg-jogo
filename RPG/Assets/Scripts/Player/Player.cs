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


    void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        UpdateHUD();
    }

    public void UpdateHUD() {
        UIController.HUD.UpdateVida(vida, vidaMax);
        UIController.HUD.UpdateCalor(calor, calorMax);
        UIController.HUD.UpdateAtributos(dano, defesa, velocidade);
        UIController.HUD.UpdatePecas(pecas);
    }

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

            if (item.tipo == Item.Tipo.Consumivel) {
                Consumivel consumivel = item.GetConsumivel();
                if (consumivel != null) consumivel.Use();
                Destroy(other.gameObject);
            }
        }
    }
}
