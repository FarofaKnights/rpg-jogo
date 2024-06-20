using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaoCalor : Consumivel {
    public int calor;

    public override void Use() {
        Player.instance.AumentarCalor(calor);
    }
}
