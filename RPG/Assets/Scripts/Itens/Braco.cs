using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Braco : Equipamento {
    public AnimatorOverrideController animatorOverride;
    public int calorNecessario;

    public void Ativar() {
        if (Player.instance.calor < calorNecessario) {
            Debug.Log("Calor insuficiente!");
            return;
        }

        AtivarEfeito();
        Player.instance.DiminuirCalor(calorNecessario);
    }

    protected abstract void AtivarEfeito();

    public override void Equip() {
        Player.instance.EquiparBraco(this);
    }

    public override void Unequip() {
        Player.instance.DesequiparBraco();
    }
}
