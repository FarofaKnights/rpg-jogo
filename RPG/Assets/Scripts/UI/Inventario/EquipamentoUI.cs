using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaEquipamentosUI : MonoBehaviour, UITab {
    public StatUI forca, vida, calor;
    public InventarioUI armasInv, bracosInv, restoInv;
    public System.Action<ItemData> onSlotClick;

    [Header("Informações selecionado")]
    [HideInInspector] public ItemData focusedItem;
    ItemData lastFocusedItem;
    public GameObject focusItemPanel, equiparBtn;
    public Image focusedItemImage;
    public Text focusedItemName, focusedItemDescription;

    void Start() {
        InventarioUI[] inventarios = new InventarioUI[] { armasInv, bracosInv, restoInv };

        foreach (InventarioUI inventario in inventarios) {
            inventario.onSlotClick += HandleSlotClick;
            inventario.onSlotFocus += HandleSlotFocus;
        }

        Player.instance.stats.OnChange += HandleStatChange;
        UpdateStats();
        UpdateFocusedItem();
    }

    public void UpdateStats() {
        Player.instance.stats.ForEach(HandleStatChange);
    }

    void HandleStatChange(string stat, int value) {
        switch (stat) {
            case "forca":
                forca.SetStat(value);
                break;
            case "vida":
                vida.SetStat(value);
                break;
            case "calor":
                calor.SetStat(value);
                break;
        }
    }

    void HandleSlotClick(ItemData itemData) {
        onSlotClick?.Invoke(itemData);
        Player.instance.inventario.HandleSlotClick(itemData);
    }

    public void HandleEquiparBtn() {
        if (focusedItem != null) {
            onSlotClick?.Invoke(focusedItem);
            Player.instance.inventario.HandleSlotClick(focusedItem);
        }
        
        UpdateFocusedItem();
    }

    void HandleSlotFocus(ItemData itemData) {
        lastFocusedItem = focusedItem;
        focusedItem = itemData;
        UpdateFocusedItem();
    }

    public void UpdateFocusedItem() {
        if (lastFocusedItem != null) {
            GetInventario(lastFocusedItem).UnfocusItem(lastFocusedItem);
        }

        if (focusedItem != null && !Player.instance.inventario.ContainsItem(focusedItem)) {
            focusedItem = null;
        }

        if (focusedItem == null) {
            focusedItemImage.gameObject.SetActive(false);
            focusedItemName.gameObject.SetActive(false);
            focusedItemDescription.gameObject.SetActive(false);
            focusItemPanel.SetActive(false);
        } else {
            focusedItemImage.gameObject.SetActive(true);
            focusedItemImage.sprite = focusedItem.sprite;
            focusedItemName.gameObject.SetActive(true);
            focusedItemName.text = focusedItem.nome;
            focusedItemDescription.gameObject.SetActive(true);
            focusedItemDescription.text = focusedItem.descricao;
            focusItemPanel.SetActive(true);

            if (focusedItem.tipo == ItemData.Tipo.Consumivel) {
                Consumivel consumivel = focusedItem.prefab.GetComponent<Consumivel>();
                equiparBtn.GetComponentInChildren<Text>().text = "Usar";
                equiparBtn.SetActive(consumivel.PodeUsar());
            } else if (focusedItem.tipo == ItemData.Tipo.Arma || focusedItem.tipo == ItemData.Tipo.Braco) {
                equiparBtn.GetComponentInChildren<Text>().text = "Equipar";
                equiparBtn.SetActive(!focusedItem.IsEquipped());
            } else if (focusedItem.tipo == ItemData.Tipo.Quest) {
                equiparBtn.SetActive(false);
            }

            GetInventario(focusedItem).FocusItem(focusedItem);
        }
    }

    public void RefreshUI() {
        armasInv.RefreshUI();
        bracosInv.RefreshUI();
        restoInv.RefreshUI();

        UpdateFocusedItem();
    }

    public void SetupUI(InventarioManager inventario) {
        armasInv.SetupUI(inventario.armas);
        bracosInv.SetupUI(inventario.bracos);
        restoInv.SetupUI(inventario.resto);

        UpdateFocusedItem();
    }

    public void Show() {
        gameObject.SetActive(true);

        SetupUI(Player.instance.inventario);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public InventarioUI GetInventario(ItemData item) {
        switch (item.tipo) {
            case ItemData.Tipo.Arma:
                return armasInv;
            case ItemData.Tipo.Braco:
                return bracosInv;
            default:
                return restoInv;
        }
    }
}
