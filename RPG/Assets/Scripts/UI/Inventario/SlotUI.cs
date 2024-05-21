using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour {
    public Text texto;
    public Image imagem;
    Outline outline;
    public GameObject textoHolder;
    [HideInInspector] public ItemData item;
    [HideInInspector] public int quantidade;

    InventarioUI inventarioUI;

    void Awake() {
        outline = GetComponent<Outline>();
    }

    public void Setup(InventarioUI inventarioUI) {
        this.inventarioUI = inventarioUI;
    }

    public void SetSlot(ItemData item, int quantidade) {
        this.item = item;
        this.quantidade = quantidade;

        if (outline == null) outline = GetComponent<Outline>();

        if (item != null && quantidade > 0) {
            imagem.gameObject.SetActive(true);
            imagem.sprite = item.sprite;
            textoHolder.SetActive(quantidade > 1);
            texto.text = quantidade.ToString();

            // Checa se é um item equipado (arma ou braço) e muda o tamanho da borda
            if (item.CheckInstance(Player.instance.arma) || item.CheckInstance(Player.instance.braco)) outline.effectDistance = new Vector2(2, -2);
            else outline.effectDistance = new Vector2(1, -1);


            
        } else {
            outline.effectDistance = new Vector2(1, -1);
            imagem.gameObject.SetActive(false);
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
