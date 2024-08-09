using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Defective.JSON;

public interface IInventario {
    bool AddItem(ItemData item, int quantidade = 1);
    bool RemoveItem(ItemData item, int quantidade = 1);
    bool ContainsItem(ItemData item);
    int GetQuantidade(ItemData item);
    void ForEach(Action<ItemData, int> action);
}

public class Inventario : Inventario<Slot> {
    public Inventario() : base() {}
}

public class Inventario<T> : IInventario, Saveable where T: Slot , new() {
    List<T> slots;
    public Action<ItemData, int> onItemChange;

    public Inventario() {
        slots = new List<T>();
    }

    public bool AddItem(ItemData item, int quantidade = 1) {
        T slot;

        if (item.empilhavel) {
            slot = GetSlotWithItem(item);
            if (slot != null) {
                slot.AddItem(item, quantidade);
                
                if (onItemChange != null)  onItemChange(item, slot.quantidade);

                return true;
            }
        }

        slot = GetEmptySlot();
        if (slot != null) {
            slot.AddItem(item, quantidade);

            if (onItemChange != null)  onItemChange(item, slot.quantidade);

            return true;
        }

        return false;
    }

    public bool RemoveItem(ItemData item, int quantidade = 1) {
        T slot = GetSlotWithItem(item);
        if (slot != null) {
            slot.RemoveItem(quantidade);
            
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
        T slot = GetSlotWithItem(item);
        return slot != null;
    }

    public int GetQuantidade(ItemData item) {
        T slot = GetSlotWithItem(item);
        if (slot != null) {
            return slot.quantidade;
        }
        return 0;
    }

    public void ForEach(Action<ItemData, int> action) {
        foreach (T slot in slots) {
            if (slot.item != null) {
                action(slot.item, slot.quantidade);
            }
        }
    }

    // Utility

    public void Clear() {
        ForEach((item, quantidade) => {
            RemoveItem(item, quantidade);
        });
    }

    T GetSlotWithItem(ItemData item) {
        foreach (T slot in slots) {
            if (slot.item == item) {
                return slot;
            }
        }
        return null;
    }

    T GetEmptySlot() {
        foreach (T slot in slots) {
            if (slot.item == null) {
                slot.quantidade = 0;
                return slot;
            }
        }

        T newSlot = new T();
        newSlot.quantidade = 0;
        slots.Add(newSlot);

        return newSlot;
    }

    // Save/Load
    public JSONObject Save() {
        JSONObject obj = new JSONObject();

        foreach (T slot in slots) {
            if (slot.item != null) {
                ItemData data = slot.item;
                obj.AddField(data.ToSaveString(), slot.quantidade);
            }
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        if (obj != null) {
            
            Clear();

            for (int i = 0; i < obj.list.Count; i++) {
				string itemName = obj.keys[i];
				int quantidade = obj.list[i].intValue;

                ItemData item = ItemManager.instance.GetItemData(itemName);
                AddItem(item, quantidade);
			}
        }
    }
}
