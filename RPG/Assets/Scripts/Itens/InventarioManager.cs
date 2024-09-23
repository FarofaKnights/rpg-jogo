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
        // O código de baixo dava erro justamente por rodar antes do UIController ser instanciado
        // Então coloquei para o equipamento chamar o HandleSlotClick apartir do próprio código
        // UIController.equipamentos.onSlotClick += HandleSlotClick;

        armas.onItemChange += HandleItemChange;
        bracos.onItemChange += HandleItemChange;
        resto.onItemChange += HandleItemChange;

        Player.instance.StartCoroutine(SkipOneStartFrame());
    }

    IEnumerator SkipOneStartFrame() {
        yield return null;
        UIController.HUD.SetArmaEquipada(Player.instance.arma);
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

    public void EquiparArma(Arma arma) {
        if (Player.instance.arma != null) DesequiparArma();

        Player.instance.arma = arma;
        arma.transform.SetParent(Player.instance.mao);
        arma.transform.localPosition = Vector3.zero;
        arma.transform.localRotation = Quaternion.identity;
        // arma.onAttackEnd += OnAttackEnded;

        UIController.equipamentos.RefreshUI();
        UIController.HUD.SetArmaEquipada(arma);
    }

    public void DesequiparArma() {
        if (Player.instance.arma == null) return;

        // arma.onAttackEnd -= OnAttackEnded;
        GameObject.Destroy(Player.instance.arma.gameObject);
        Player.instance.arma = null;
        UIController.equipamentos.RefreshUI();
        UIController.HUD.SetArmaEquipada(null);

        if (Player.instance.stateMachine.GetCurrentState() == Player.instance.attackState) {
            Player.instance.stateMachine.SetState(Player.instance.moveState);
        }
    }

    public void EquiparBraco(Braco braco) {
        if (Player.instance.braco != null) DesequiparBraco();

        Player.instance.braco = braco;
        braco.transform.SetParent(Player.instance.bracoHolder);
        braco.transform.localPosition = Vector3.zero;
        braco.transform.localRotation = Quaternion.identity;

        UIController.equipamentos.RefreshUI();
    }

    public void DesequiparBraco() {
        if (Player.instance.braco == null) return;

        GameObject.Destroy(Player.instance.braco.gameObject);
        Player.instance.braco = null;

        UIController.equipamentos.RefreshUI();
    }

    #region IInventario implementation

    public bool AddItem(ItemData item, int quantidade = 1) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.AddItem(item, quantidade);
    }

    public bool RemoveItem(ItemData item, int quantidade = 1) {
        Inventario inventario = GetInventario(item.tipo);
        return inventario.RemoveItem(item, quantidade);
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

    public bool HandleSlotClick(ItemData itemData) {
        if (itemData == null) return false;

        TipoAbstrato especificacoes = itemData.prefab.GetComponent<TipoAbstrato>();
        if (!especificacoes.IsUsavel) return false;

        TipoAbstrato instancia = especificacoes;

        if (itemData.tipo == ItemData.Tipo.Consumivel) {
            Consumivel consumivel = (Consumivel) especificacoes;
            if (!consumivel.PodeUsar()) return false;

            RemoveItem(itemData);
        }

        if (especificacoes.IsInstanciavel) {
            GameObject obj = itemData.CreateInstance();
            Item item = obj.GetComponent<Item>();
            instancia = item.GetTipo();
        }

        instancia.FazerAcao();
        return true;
    }

    #endregion

    #region Saveable implementation
    
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

    #endregion

}
