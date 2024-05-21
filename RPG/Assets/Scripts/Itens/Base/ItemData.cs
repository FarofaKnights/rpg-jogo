using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "RPG/ItemData")]
public class ItemData : ScriptableObject {
    public enum Tipo {Consumivel, Quest, Braco, Arma }

    public Tipo tipo;
    public string nome;
    public Sprite sprite;
    public string descricao;
    public bool empilhavel = true;
    public GameObject prefab;

    public GameObject CreateInstance() {
        return Instantiate(prefab.gameObject);
    }

    public bool CheckInstance(Item item) {
        if (item == null) return false;
        return item.data == this;
    }

    public bool CheckInstance(GameObject objeto) {
        // Isso não é totalmente verdade, uma vez que o TipoAbstrato pode estar com um compartamento diferente do prefab, porém, para o propósito do jogo, é suficiente
        if (objeto == null) return false;
        if (objeto.GetComponent<TipoAbstrato>() == null) return false;

        return CheckInstance(objeto.GetComponent<Item>());
    }

    public bool CheckInstance(TipoAbstrato tipo) {
        if (tipo == null) return false;
        return CheckInstance(tipo.GetComponent<Item>());
    }
}
