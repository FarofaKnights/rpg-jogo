using UnityEngine;

public class Slot : MonoBehaviour {
    public Item item;
    public int quantidade = 1;

    public void AddItem(Item novoItem) {
        if (item == null) {
            item = novoItem;
            quantidade = 1;
        } else if (item == novoItem && item.empilhavel) {
            quantidade++;
        }
    }

    public void RemoveItem() {
        if (quantidade > 0) {
            quantidade--;
        }
        if (quantidade == 0) {
            item = null;
        }
    }
}
