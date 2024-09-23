using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaoCura : Consumivel {
    public int cura;

    public override void Use() {
        Player.Atributos.vida.Add(cura);
    }

    public override bool PodeUsar() {
        return Player.instance.atributos.vida.Get() < Player.instance.atributos.vida.GetMax();
    }
}
