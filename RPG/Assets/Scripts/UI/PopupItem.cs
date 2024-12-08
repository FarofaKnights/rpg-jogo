using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupItem : MonoBehaviour {
    public float timeOnScreen = 2.0f;
    float currentTimeOnScreen = 0.0f;

    [HideInInspector] public ItemData itemData;
    public Text texto;
    public Image imagem;
    int quant = 0;

    public System.Action<ItemData, int> OnEnd;

    public void Set(ItemData itemData, int quant) {
        string escrito = quant >= 0 ? "+" : "";
        escrito += quant + " " + itemData.nome;

        this.itemData = itemData;
        this.imagem.sprite = itemData.sprite;
        this.texto.text = escrito;
        this.quant = quant;

        currentTimeOnScreen = 0.0f;
    }

    public void UpdateValue(int quant) {
        this.quant += quant;
        Set(this.itemData, this.quant);
    }

    void FixedUpdate() {
        currentTimeOnScreen += Time.fixedDeltaTime;
        if (currentTimeOnScreen >= timeOnScreen) {
            Destroy(gameObject);
            OnEnd?.Invoke(itemData, quant);
        }
    }
}
