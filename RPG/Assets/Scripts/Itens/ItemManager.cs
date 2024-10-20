using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager {
    public static ItemManager instance { get { return GameManager.instance.itemManager; } }

    // Save string format example: "Consumivel/PocaoCuraData"

    public ItemData GetItemData(string saveString) {
        string[] split = saveString.Split('/');

        if (split.Length != 2) {
            Debug.LogError("Save string inválida: " + saveString);
            return null;
        }

        string tipo = split[0];
        string name = split[1];

        return GetItemData(name, tipo);
    }

    public ItemData GetItemData(string itemDataName, ItemData.Tipo tipo) {
        string tipoPath = ItemData.TipoToStringPath(tipo);
        return GetItemData(itemDataName, tipoPath);
    }

    public ItemData GetItemData(string itemDataName, string tipoPath) {
        string path = "Itens/" + tipoPath + "/Data/" + itemDataName;
        ItemData itemData = Resources.Load<ItemData>(path);
        if (itemData == null) {
            Debug.LogError("ItemData não encontrado em " + path);
            return null;
        }
        return itemData;
    }

}
