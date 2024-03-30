using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Equipamento : TipoAbstrato {
    public abstract void Equip();
    public abstract void Unequip();
    public override void FazerAcao() {
        Equip();
    }
}