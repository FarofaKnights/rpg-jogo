using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : Equipamento {
    public float dano;
    
    public override void Equip() {
        Player.instance.EquiparArma(this);
    }

    public override void Unequip() {
        Player.instance.DesequiparArma();
    }
}
