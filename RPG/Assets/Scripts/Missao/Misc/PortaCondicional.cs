using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class PortaCondicional : MonoBehaviour {
    public GameObject porta;

    public CondicaoInfo condicaoInfo;

    Condicao condicao;

    public bool consumirItem = false;

    void Start() {
        condicao = condicaoInfo.GetCondicao();
        condicao.Then(() => {
            if (consumirItem && condicao.GetType() == typeof(CondicaoTemItem)) {
                CondicaoTemItem cond = (CondicaoTemItem)condicao;
                Player.instance.inventario.RemoveItem(cond.itemData, cond.quantidade);
            }

            Destroy(porta);
        });

        if (GetComponent<Interagivel>() != null) {
            Interagivel inte = GetComponent<Interagivel>();
            if (inte.tagFilter == null) inte.tagFilter = "Player";
        }
    }

    public void OnPlayerInteract() {
        condicao.Realizar();
    }
}
