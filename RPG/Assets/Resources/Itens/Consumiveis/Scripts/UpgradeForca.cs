using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeForca : Consumivel {
    public override void Use() {
        Player.instance.stats.AddForca(1);
    }
}
