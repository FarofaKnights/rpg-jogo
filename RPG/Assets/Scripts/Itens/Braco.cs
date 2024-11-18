using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Braco : Equipamento {
    public AnimatorOverrideController animatorOverride;
    public int calorNecessario;
    public Transform baseBraco, meioBraco, maoBraco;

    public void Ativar() {
        if (equipador.GetInfo().gameObject != Player.instance.gameObject) {
            AtivarEfeito();
            return;
        }

        if (!PodeAtivar()) {
            Debug.Log("Calor insuficiente!");
            return;
        }

        AtivarEfeito();
        Player.Atributos.calor.Sub(calorNecessario);
    }

    protected abstract void AtivarEfeito();

    public bool IsEquipped() {
        return Player.instance.braco == this;
    }

    public bool PodeAtivar() {
        return Player.Atributos.calor.Get() >= calorNecessario && IsEquipped();
    }
}
