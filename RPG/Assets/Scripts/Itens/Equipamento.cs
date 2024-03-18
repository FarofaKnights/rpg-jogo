using UnityEngine;

public class Equipamento : Item {
    [Header("Atributos do Equipamento")]
    public int ataque;
    public int defesa;
    public int velocidade;

    public virtual void Equip() {
        Debug.Log("Equipando " + nome);
    }

    public virtual void Unequip() {
        Debug.Log("Desequipando " + nome);
    }

    void Awake() {
        empilhavel = false;
    }
}