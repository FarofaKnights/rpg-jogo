using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondicaoTemItem : Condicao {
    public ItemData itemData;
    public int quantidade = 1;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "intValue", "dinamic" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Path do Item", "Quantidade", "É dinâmico" }; }
    

    public CondicaoTemItem(ItemData itemData, int quantidade) {
        this.itemData = itemData;
        this.quantidade = quantidade;

        if (this.dinamica) {
            Realizar(); // Tenta realizar a condição logo de cara
            Player.instance.inventario.OnChange += HandleItemChange;
        }
    }

    public CondicaoTemItem(string itemDataId, int quantidade): this(ItemManager.instance.GetItemData(itemDataId), quantidade) { }

    public CondicaoTemItem(CondicaoParams parameters): base(parameters) {
        this.itemData = ItemManager.instance.GetItemData(parameters.id);
        this.quantidade = parameters.intValue;

        if (this.dinamica) {
            Realizar(); // Tenta realizar a condição logo de cara
            Player.instance.inventario.OnChange += HandleItemChange;
        }
    }

    void HandleItemChange(ItemData item, int quantidade) {
        if (item == itemData && quantidade >= this.quantidade) {
            Player.instance.inventario.OnChange -= HandleItemChange;
            Realizar();
        }
    }

    public override bool CheckCondicao() {
        if (!Player.instance.inventario.ContainsItem(itemData)) return false;
        return Player.instance.inventario.GetQuantidade(itemData) >= quantidade;
    }
}
