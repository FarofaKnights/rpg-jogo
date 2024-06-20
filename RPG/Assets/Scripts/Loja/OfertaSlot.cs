using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OfertaSlot : Slot {
    public bool infinito = false;
    public int preco = 0;

    public override void RemoveItem(int quant = 1) {
        ItemData item = this.item;
        if (!infinito) base.RemoveItem(quant);
        if (this.item == null) this.item = item; 
    }
}
