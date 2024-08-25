using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour {
    public Text texto;
    public Image imagem, background;
    public GameObject selectedIndicator;
    Outline outline;
    public GameObject textoHolder;
    [HideInInspector] public ItemData item;
    [HideInInspector] public int quantidade;

    public Color normalOutlineColor, selectedOutlineColor;
    public Color normalBackgroundColor, focusBackgroundColor;

    InventarioUI inventarioUI;

    void Awake() {
        outline = GetComponent<Outline>();
        background = GetComponent<Image>();
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
            if (item.CheckInstance(Player.instance.arma) || item.CheckInstance(Player.instance.braco) || item.CheckInstance(Player.instance.consumivelSelecionado)) {
                outline.effectDistance = new Vector2(2, -2);
                outline.effectColor = selectedOutlineColor;
            }
            else {
                outline.effectDistance = new Vector2(1, -1);
                outline.effectColor = normalOutlineColor;
            }

        } else {
            outline.effectDistance = new Vector2(1, -1);
            outline.effectColor = normalOutlineColor;
            imagem.gameObject.SetActive(false);
            imagem.sprite = null;
            textoHolder.SetActive(false);
        }
    }

    public void OnClick() {
        if (item != null) {
            inventarioUI.HandleFocusSlot(this);
        }
    }
    public void OnSelect() {
        outline.effectColor = selectedOutlineColor;
        if (selectedIndicator != null) selectedIndicator.SetActive(true);
    }

    public void OnDeselect() {
        outline.effectColor = normalOutlineColor;
        if (selectedIndicator != null) selectedIndicator.SetActive(false);
    }

    public void OnFocus() {
        background.color = focusBackgroundColor;
    }

    public void OnUnfocus() {
        background.color = normalBackgroundColor;
    }

    

    void OnValidate() {
        outline = GetComponent<Outline>();
        background = GetComponent<Image>();

        OnDeselect();
        OnUnfocus();
    }
}
