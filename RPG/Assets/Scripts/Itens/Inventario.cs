using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Inventario {
    List<Slot> slots;
    public Action<Item, int> onItemChange;

    public Inventario() {
        slots = new List<Slot>();
    }

    public bool AddItem(Item item) {
        Slot slot;

        if (item.empilhavel) {
            slot = GetSlotWithItem(item);
            if (slot != null) {
                slot.AddItem(item);
                
                if (onItemChange != null)  onItemChange(item, slot.quantidade);

                return true;
            }
        }

        slot = GetEmptySlot();
        if (slot != null) {
            slot.AddItem(item);

            if (onItemChange != null)  onItemChange(item, slot.quantidade);

            return true;
        }

        return false;
    }

    public bool RemoveItem(Item item) {
        Slot slot = GetSlotWithItem(item);
        if (slot != null) {
            slot.RemoveItem();
            
            if (onItemChange != null)  onItemChange(item, slot.quantidade);

            return true;
        }
        return false;
    }

    public int GetSlotIndex(Item item) {
        for (int i = 0; i < slots.Count; i++) {
            if (slots[i].item == item) {
                return i;
            }
        }
        return -1;
    }

    public Item GetItemInSlot(int index) {
        if (index < 0 || index >= slots.Count) {
            return null;
        }

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

    public void ForEach(Action<Item, int> action) {
        foreach (Slot slot in slots) {
            if (slot.item != null) {
                action(slot.item, slot.quantidade);
            }
        }
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

        Slot newSlot = new Slot();
        newSlot.quantidade = 0;
        slots.Add(newSlot);

        return newSlot;
    }
}
