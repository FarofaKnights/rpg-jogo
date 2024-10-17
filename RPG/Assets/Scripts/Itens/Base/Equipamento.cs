using UnityEngine;

public interface IEquipador : IAtacador {
    void Equipar(Equipamento equipamento);
    void Desequipar(Equipamento equipamento);
}

[RequireComponent(typeof(Item))]
public abstract class Equipamento : TipoAbstrato {
    public IEquipador equipador;

    public virtual void Equip(IEquipador equipador) {
        this.equipador = equipador;
        equipador.Equipar(this);
    }

    public virtual void Unequip(IEquipador equipador) {
        equipador.Desequipar(this);
        this.equipador = null;
    }

    public override void FazerAcao() {
        Equip(Player.instance);
    }
}