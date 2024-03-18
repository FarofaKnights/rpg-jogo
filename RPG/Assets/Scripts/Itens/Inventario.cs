using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario {
    public List<Slot> slots;

    public bool AddItem(Item item) {
        Slot slot;

        if (item.empilhavel) {
            slot = GetSlotWithItem(item);
            if (slot != null) {
                slot.AddItem(item);
                return true;
            }
        }

        slot = GetEmptySlot();
        if (slot != null) {
            slot.AddItem(item);
            return true;
        }

        return false;
    }

    public bool RemoveItem(Item item) {
        Slot slot = GetSlotWithItem(item);
        if (slot != null) {
            slot.RemoveItem();
            return true;
        }
        return false;
    }

    public Item GetItemInSlot(int index) {
        return slots[index].item;
    }

    public bool ContainsItem(Item item) {
        Slot slot = GetSlotWithItem(item);
        return slot != null;
    }

    public int GetQuantidade(Item item) {
        Slot slot = GetSlotWithItem(item);
        if (slot != null) {
            return slot.quantidade;
        }
        return 0;
    }

    // Utility

    Slot GetSlotWithItem(Item item) {
        foreach (Slot slot in slots) {
            if (slot.item == item) {
                return slot;
            }
        }
        return null;
    }

    Slot GetEmptySlot() {
        foreach (Slot slot in slots) {
            if (slot.item == null) {
                slot.quantidade = 0;
                return slot;
            }
        }
        return null;
    }
}
