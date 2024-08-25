using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventarioUI : MonoBehaviour {
    public Action<ItemData> onSlotClick, onSlotFocus;
    public GameObject slotPrefab;
    public int minSlots = 1;

    List<SlotUI> slotsUI;
    Inventario inventario;

    void Awake() {
        // Clear all children
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void RefreshUI() {
        SetupUI(inventario);
    }

    AreaEquipamentosUI parent;
    AreaEquipamentosUI GetParent() {
        if (parent == null) parent = GetComponentInParent<AreaEquipamentosUI>();
        return parent;
    }

    void ClearUI() {
        if (this.inventario != null)
            this.inventario.onItemChange -= UpdateSlot;
        
        if (slotsUI != null) {
            slotsUI.ForEach(slot => Destroy(slot.gameObject));
            slotsUI.Clear();
        }
        
        // Clear all children
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void SetupUI(Inventario inventario) {
        if (inventario == null) {
            ClearUI();
            return;
        }

        if (this.inventario != null) {
            ClearUI();
        }

        this.inventario = inventario;
        
        slotsUI = new List<SlotUI>();
        inventario.ForEach((ItemData item, int quantidade) => CreateSlotUI(item, quantidade));

        if (slotsUI.Count < minSlots) {
            for (int i = slotsUI.Count; i < minSlots; i++) {
                CreateEmptySlotUI();
            }
        }

        inventario.onItemChange += UpdateSlot;
    }

    public void UpdateSlot(ItemData item, int quantidade) {
        SlotUI slotUI = slotsUI.Find(slot => slot.item == item);

        if (slotUI == null) {
            if (quantidade > 0) {
                slotUI = GetEmptySlotUI();
                slotUI.SetSlot(item, quantidade);
            }
        }
        else {
            slotUI.SetSlot(item, quantidade);
            if (quantidade <= 0) {
                slotsUI.Remove(slotUI);
                Destroy(slotUI.gameObject);

                if (slotsUI.Count < minSlots) {
                    CreateEmptySlotUI();
                }
            }
        }
    }

    public void HandleSlotClick(ItemData item) {
        if (item != null) {
            onSlotClick?.Invoke(item);
        }
    }

    public void HandleFocusSlot(SlotUI slot) {
        onSlotFocus?.Invoke(slot.item);
    }

    public void UnfocusItem(ItemData item) {
        SlotUI slot = slotsUI.Find(s => s.item == item);
        if (slot != null) {
            slot.OnUnfocus();
        }
    }

    public void FocusItem(ItemData item) {
        SlotUI slot = slotsUI.Find(s => s.item == item);
        if (slot != null) {
            slot.OnFocus();
        }
    }

    // Utility
    SlotUI CreateSlotUI(ItemData item, int quantidade) {
        GameObject slotGO = Instantiate(slotPrefab, transform);
        SlotUI slotUI = slotGO.GetComponent<SlotUI>();
        slotUI.Setup(this);
        slotUI.SetSlot(item, quantidade);
        slotsUI.Add(slotUI);
        return slotUI;
    }

    SlotUI CreateEmptySlotUI() {
        return CreateSlotUI(null, 0);
    }

    SlotUI GetEmptySlotUI() {
        SlotUI slotUI = slotsUI.Find(slot => slot.item == null);
        if (slotUI == null) {
            slotUI = CreateEmptySlotUI();
        }
        return slotUI;
    }
}
