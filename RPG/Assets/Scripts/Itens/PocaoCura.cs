using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaoCura : Consumivel {
    public int cura;

    public override void Use() {
        Player.instance.CurarVida(cura);
    }
}
