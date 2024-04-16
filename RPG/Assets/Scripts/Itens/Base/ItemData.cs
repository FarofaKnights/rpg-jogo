using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "RPG/ItemData")]
public class ItemData : ScriptableObject {
    public enum Tipo {Consumivel, Equipamento, Quest }

    public Tipo tipo;
    public string nome;
    public Sprite sprite;
    public string descricao;
    public bool empilhavel = true;
    public GameObject prefab;

    public GameObject CreateInstance() {
        return Instantiate(prefab.gameObject);
    }
}
