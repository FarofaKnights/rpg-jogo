using UnityEngine;

public class Item : MonoBehaviour {
    public enum Tipo {Consumivel, Equipamento, Quest }


    [Header("Informações do Item")]
    public Tipo tipo;
    public string nome;
    public Sprite sprite;
    public string descricao;
    public bool empilhavel = true;

    public Consumivel GetConsumivel() {
        if (tipo != Tipo.Consumivel) return null;

        return GetComponent<Consumivel>();
    }
}