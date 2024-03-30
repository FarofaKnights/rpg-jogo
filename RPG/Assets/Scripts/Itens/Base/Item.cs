using UnityEngine;

public class Item : MonoBehaviour {
    public ItemData data;

    public TipoAbstrato GetTipo() {
        if (data.tipo == ItemData.Tipo.Consumivel) return GetComponent<Consumivel>();
        else if (data.tipo == ItemData.Tipo.Equipamento) return GetComponent<Equipamento>();
        else if (data.tipo == ItemData.Tipo.Quest) return GetComponent<QuestItem>();
        else return null;
    }
}