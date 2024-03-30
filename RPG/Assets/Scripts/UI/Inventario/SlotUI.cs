using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour {
    public Text texto;
    public Image imagem;
    public GameObject textoHolder;
    [HideInInspector] public ItemData item;
    [HideInInspector] public int quantidade;

    InventarioUI inventarioUI;

    public void Setup(InventarioUI inventarioUI) {
        this.inventarioUI = inventarioUI;
    }

    public void SetSlot(ItemData item, int quantidade) {
        this.item = item;
        this.quantidade = quantidade;

        if (item != null && quantidade > 0) {
            imagem.sprite = item.sprite;
            textoHolder.SetActive(quantidade > 1);
            texto.text = quantidade.ToString();
        } else {
            imagem.sprite = null;
            textoHolder.SetActive(false);
        }
    }

    public void OnClick() {
        if (item != null) {
            inventarioUI.HandleSlotClick(this.item);
        }
    }
}
