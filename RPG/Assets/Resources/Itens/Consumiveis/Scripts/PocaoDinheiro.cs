using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaoDinheiro : Consumivel {
    public int pecas;

    public override void Use() {
        Player.Atributos.pecas.Add(pecas);
    }
}
