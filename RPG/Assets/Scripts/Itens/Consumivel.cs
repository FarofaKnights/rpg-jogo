using UnityEngine;

public class Consumivel : Item {
    [Header("Atributos do Consumível")]
    public int cura;
    public int mana;

    public virtual void Use() {
        Debug.Log("Usando " + nome);
    }
}