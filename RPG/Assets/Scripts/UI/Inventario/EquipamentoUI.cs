using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEquipamentosUI : MonoBehaviour, UITab {
    public StatUI forca, vida, calor;
    public InventarioUI armasInv, bracosInv, restoInv;
    public System.Action<ItemData> onSlotClick;

    void Start() {
        armasInv.onSlotClick += HandleSlotClick;
        bracosInv.onSlotClick += HandleSlotClick;
        restoInv.onSlotClick += HandleSlotClick;

        Player.instance.stats.OnChange += HandleStatChange;
        UpdateStats();
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

    public void RefreshUI() {
        armasInv.RefreshUI();
        bracosInv.RefreshUI();
        restoInv.RefreshUI();
    }

    public void SetupUI(InventarioManager inventario) {
        armasInv.SetupUI(inventario.armas);
        bracosInv.SetupUI(inventario.bracos);
        restoInv.SetupUI(inventario.resto);
    }

    public void Show() {
        gameObject.SetActive(true);

        SetupUI(Player.instance.inventario);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
