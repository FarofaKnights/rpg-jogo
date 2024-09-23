using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaoCalor : Consumivel {
    public int calor;

    public override void Use() {
        Player.Atributos.calor.Add(calor);
    }

    public override bool PodeUsar() {
        return Player.instance.atributos.calor.Get() < Player.instance.atributos.calor.GetMax();
    }
}
