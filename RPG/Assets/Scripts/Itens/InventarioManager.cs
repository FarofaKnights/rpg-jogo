using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public class InventarioManager : IInventario, Saveable {
    public Inventario armas, bracos, resto;
    public System.Action<ItemData, int> OnChange;

    public InventarioManager() {
        armas = new Inventario();
        bracos = new Inventario();
        resto = new Inventario();

        // UIController.inventarioUI.SetupUI(inventario);
        UIController.equipamentos.onSlotClick += HandleSlotClick;

        armas.onItemChange += HandleItemChange;
        bracos.onItemChange += HandleItemChange;
        resto.onItemChange += HandleItemChange;
    }

    void HandleItemChange(ItemData item, int quantidade) {
        if (OnChange != null) {
            OnChange(item, quantidade);
        }
    }

    public Inventario GetInventario(ItemData.Tipo tipo) {
        if (tipo == ItemData.Tipo.Braco) {
            return bracos;
        } else if (tipo == ItemData.Tipo.Arma) {
            return armas;
        } else {
            return resto;
        }
    }

    public bool AddItem(ItemData item) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.AddItem(item);
    }

    public bool RemoveItem(ItemData item) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.RemoveItem(item);
    }

    public bool ContainsItem(ItemData item) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.ContainsItem(item);
    }

    public int GetQuantidade(ItemData item) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.GetQuantidade(item);
    }

    public void ForEach(System.Action<ItemData, int> action) {
        armas.ForEach(action);
        bracos.ForEach(action);
        resto.ForEach(action);
    }

    public void HandleSlotClick(ItemData itemData) {
        if (itemData == null) return;

        TipoAbstrato especificacoes = itemData.prefab.GetComponent<TipoAbstrato>();
        if (!especificacoes.IsUsavel) return;

        TipoAbstrato instancia = especificacoes;

        if (itemData.tipo == ItemData.Tipo.Consumivel) {
            RemoveItem(itemData);
        }

        if (especificacoes.IsInstanciavel) {
            GameObject obj = itemData.CreateInstance();
            Item item = obj.GetComponent<Item>();
            instancia = item.GetTipo();
        }

        instancia.FazerAcao();
    }
    
    public JSONObject Save() {
        JSONObject obj = new JSONObject();
        JSONObject armasObj = armas.Save();
        JSONObject bracosObj = bracos.Save();
        JSONObject restoObj = resto.Save();

        if (armasObj.list != null && armasObj.list.Count > 0) {
            obj.AddField("armas", armasObj);
        }

        if (bracosObj.list != null && bracosObj.list.Count > 0) {
            obj.AddField("bracos", bracosObj);
        }

        if (restoObj.list != null && restoObj.list.Count > 0) {
            obj.AddField("resto", restoObj);
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        JSONObject armasObj = obj.GetField("armas");
        JSONObject bracosObj = obj.GetField("bracos");
        JSONObject restoObj = obj.GetField("resto");

        if (armasObj != null) {
            armas.Load(armasObj);
        }

        if (bracosObj != null) {
            bracos.Load(bracosObj);
        }

        if (restoObj != null) {
            resto.Load(restoObj);
        }
    }
}
