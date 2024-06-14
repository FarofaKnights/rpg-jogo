using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoDaItem : Acao {
    public ItemData itemData;
    public int quantidade = 1;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "intValue" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Path do Item", "Quantidade" }; }

    public AcaoDaItem(ItemData itemData, int quantidade) {
        this.itemData = itemData;
        this.quantidade = quantidade;
    }

    public AcaoDaItem(string itemDataId, int quantidade): this(ItemManager.instance.GetItemData(itemDataId), quantidade) { }

    public AcaoDaItem(CondicaoParams parameters): base(parameters) {
        this.itemData = ItemManager.instance.GetItemData(parameters.id);
        this.quantidade = parameters.intValue;
    }

    public override void Realizar() {
        Player.instance.inventario.AddItem(itemData, quantidade);
    }
}
