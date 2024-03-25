using UnityEngine;

public class Equipamento : TipoAbstrato {
    [Header("Atributos do Equipamento")]
    public int ataque;
    public int defesa;
    public int velocidade;

    public virtual void Equip() {
        Debug.Log("Equipando ");
    }

    public virtual void Unequip() {
        Debug.Log("Desequipando ");
    }

    public override void FazerAcao() {
        Equip();
    }
}