using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventarioManager : IInventario {
    public Inventario armas, bracos, resto;

    public InventarioManager() {
        armas = new Inventario();
        bracos = new Inventario();
        resto = new Inventario();

        // UIController.inventarioUI.SetupUI(inventario);
        UIController.equipamentos.onSlotClick += HandleSlotClick;
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
    
}
