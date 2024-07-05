using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Braco : Equipamento {
    public AnimatorOverrideController animatorOverride;
    public int calorNecessario;

    public void Ativar() {
        if (Player.Atributos.calor.Get() < calorNecessario) {
            Debug.Log("Calor insuficiente!");
            return;
        }

        AtivarEfeito();
        Player.Atributos.calor.Sub(calorNecessario);
    }

    protected abstract void AtivarEfeito();

    public override void Equip() {
        Player.Inventario.EquiparBraco(this);
    }

    public override void Unequip() {
        Player.Inventario.DesequiparBraco();
    }
}
