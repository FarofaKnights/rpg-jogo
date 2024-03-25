using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventarioUI : MonoBehaviour {
    public Action<Item> onSlotClick;
    public GameObject slotPrefab;
    List<SlotUI> slotsUI;
    Inventario inventario;

    void Awake() {
        // Clear all children
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void SetupUI(Inventario inventario) {
        if (this.inventario != null) {
            this.inventario.onItemChange -= UpdateSlot;
            slotsUI.ForEach(slot => Destroy(slot.gameObject));
            slotsUI.Clear();

            // Clear all children
            for (int i = transform.childCount - 1; i >= 0; i--) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        this.inventario = inventario;
        
        slotsUI = new List<SlotUI>();
        inventario.ForEach(CreateSlotUI);
        inventario.onItemChange += UpdateSlot;
    }

    public void UpdateSlot(Item item, int quantidade) {
        SlotUI slotUI = slotsUI.Find(slot => slot.item == item);

        if (slotUI == null) CreateSlotUI(item, quantidade);
        else {
            slotUI.SetSlot(item, quantidade);
            if (quantidade <= 0) {
                slotsUI.Remove(slotUI);
                Destroy(slotUI.gameObject);
            }
        }
    }

    public void HandleSlotClick(Item item) {
        if (item != null) {
            onSlotClick?.Invoke(item);
        }
    }

    // Utility
    void CreateSlotUI(Item item, int quantidade) {
        GameObject slotGO = Instantiate(slotPrefab, transform);
        SlotUI slotUI = slotGO.GetComponent<SlotUI>();
        slotUI.Setup(this);
        slotUI.SetSlot(item, quantidade);
        slotsUI.Add(slotUI);
    }
}
