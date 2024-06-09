using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropado : MonoBehaviour {
    public Item item;
    public int quantidade = 1;
    // public bool seUsuarioNaoTiver = false; // Garante que o item só esteja disponível se o jogador não tiver ele no inventário

    void Start() {
        if (item == null) {
            Debug.LogError("ItemDropado sem item definido!");
            Destroy(gameObject);
        }

        if (quantidade <= 0) {
            Debug.LogWarning("ItemDropado com quantidade menor ou igual a zero, destruindo objeto.");
            Destroy(gameObject);
        }
    }
}
