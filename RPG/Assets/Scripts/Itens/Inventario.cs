using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Defective.JSON;

public interface IInventario {
    bool AddItem(ItemData item);
    bool RemoveItem(ItemData item);
    bool ContainsItem(ItemData item);
    int GetQuantidade(ItemData item);
    void ForEach(Action<ItemData, int> action);
}

public class Inventario : IInventario, Saveable{
    List<Slot> slots;
    public Action<ItemData, int> onItemChange;

    public Inventario() {
        slots = new List<Slot>();
    }

    public bool AddItem(ItemData item) {
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

    public bool RemoveItem(ItemData item) {
        Slot slot = GetSlotWithItem(item);
        if (slot != null) {
            slot.RemoveItem();
            
            if (onItemChange != null)  onItemChange(item, slot.quantidade);

            return true;
        }
        return false;
    }

    public int GetSlotIndex(ItemData item) {
        for (int i = 0; i < slots.Count; i++) {
            if (slots[i].item == item) {
                return i;
            }
        }
        return -1;
    }

    public ItemData GetItemInSlot(int index) {
        if (index < 0 || index >= slots.Count) {
            return null;
        }

        return slots[index].item;
    }

    public bool ContainsItem(ItemData item) {
        Slot slot = GetSlotWithItem(item);
        return slot != null;
    }

    public int GetQuantidade(ItemData item) {
        Slot slot = GetSlotWithItem(item);
        if (slot != null) {
            return slot.quantidade;
        }
        return 0;
    }

    public void ForEach(Action<ItemData, int> action) {
        foreach (Slot slot in slots) {
            if (slot.item != null) {
                action(slot.item, slot.quantidade);
            }
        }
    }

    // Utility

    Slot GetSlotWithItem(ItemData item) {
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

    // Save/Load
    public JSONObject Save() {
        JSONObject obj = new JSONObject();

        foreach (Slot slot in slots) {
            if (slot.item != null) {
                ItemData data = slot.item;
                obj.AddField(data.ToSaveString(), slot.quantidade);
            }
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        if (obj != null) {
            for (int i = 0; i < obj.list.Count; i++) {
				string itemName = obj.keys[i];
				int quantidade = obj.list[i].intValue;

                ItemData item = ItemManager.instance.GetItemData(itemName);
                Slot slot = GetEmptySlot();
                slot.AddItem(item, quantidade);
			}
        }
    }
}
