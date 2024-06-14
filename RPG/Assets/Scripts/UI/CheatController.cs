using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class CheatController : MonoBehaviour {
    public Dropdown addItemDropdown, teletransporteDropdown;

    void Start() {
        GerarDropdownItens();
        GerarDropdownTeletransporte();
    }

    void GerarDropdownItens() {
        ItemList list = Resources.Load<ItemList>("Itens/TodosItens");
        List<string> opcoes = new List<string>();

        foreach (RelacaoNomeItem relacao in list.itens) {
            string nome = relacao.nome;
            ItemData item = relacao.item;
            opcoes.Add(nome);
        }

        addItemDropdown.ClearOptions();
        addItemDropdown.AddOptions(opcoes);
    }

    void GerarDropdownTeletransporte() {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        List<string> opcoes = new List<string>();

        foreach (SpawnPoint spawnPoint in spawnPoints) {
            string nome = spawnPoint.pointName;
            opcoes.Add(nome);
        }

        teletransporteDropdown.ClearOptions();
        teletransporteDropdown.AddOptions(opcoes);
    }

    ItemData GetItemDataFromTodos(string nome) {
        ItemList list = Resources.Load<ItemList>("Itens/TodosItens");
        foreach (RelacaoNomeItem relacao in list.itens) {
            if (relacao.nome == nome)
                return relacao.item;
        }

        return null;
    }

    public void AddItem() {
        int i = addItemDropdown.value;

        string id = addItemDropdown.options[i].text;
        ItemData data = GetItemDataFromTodos(id);   

        Player.instance.inventario.AddItem(data);
    }

    public void Teletransportar() {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        int i = teletransporteDropdown.value;
        string nome = teletransporteDropdown.options[i].text;

        foreach (SpawnPoint spawnPoint in spawnPoints) {
            if (nome == spawnPoint.pointName){
                Player.instance.TeleportTo(spawnPoint.transform.position);
            }
        }
    }
}
