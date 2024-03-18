using UnityEngine;

public abstract class Item: MonoBehaviour {
    [Header("Informações do Item")]
    public string nome;
    public Sprite sprite;
    public string descricao;
    public bool empilhavel = true;
}