using UnityEngine;

public class Item : MonoBehaviour {
    public enum Tipo {Consumivel, Equipamento, Quest }


    [Header("Informações do Item")]
    public Tipo tipo;
    public string nome;
    public Sprite sprite;
    public string descricao;
    public bool empilhavel = true;

    public TipoAbstrato GetTipo() {
        if (tipo == Tipo.Consumivel) return GetComponent<Consumivel>();
        else if (tipo == Tipo.Equipamento) return GetComponent<Equipamento>();
        else if (tipo == Tipo.Quest) return GetComponent<QuestItem>();
        else return null;
    }
}