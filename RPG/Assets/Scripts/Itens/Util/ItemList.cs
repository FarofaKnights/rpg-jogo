using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RelacaoNomeItem {
    public string nome;
    public ItemData item;
}

[CreateAssetMenu(fileName = "Lista de Itens", menuName = "RPG/ItemList")]
public class ItemList : ScriptableObject {
    public List<RelacaoNomeItem> itens;
}
