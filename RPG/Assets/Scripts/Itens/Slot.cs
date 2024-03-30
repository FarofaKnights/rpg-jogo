using UnityEngine;

public class Slot {
    public ItemData item;
    public int quantidade = 1;

    public void AddItem(ItemData novoItem, int quant = 1) {
        if (novoItem == null) {
            Debug.LogWarning("Tentativa de adicionar item nulo ao slot");
            return;
        } else if (quant < 1) {
            Debug.LogWarning("Tentativa de adicionar item '" + novoItem.nome + "' com quantidade menor que 1");
            return;
        }

        if (item == null) {
            item = novoItem;
            quantidade = quant;
        } else if (item == novoItem && item.empilhavel) {
            quantidade += quant;
        }
    }

    public void RemoveItem(int quant = 1) {
        if (quant < 1) {
            Debug.LogWarning("Tentativa de remover item '" + item.nome + "' com quantidade menor que 1");
            return;
        }

        if (quantidade > 0) {
            quantidade -= quant;
        }

        if (quantidade <= 0) {
            item = null;
            quantidade = 0;
        }
    }
}
