using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStat : Consumivel {
    public override void Use() {
        UIController.statChoice.MostrarEscolha(1);
    }
}
