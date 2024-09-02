using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitboxAttackBehaviour : AttackBehaviour {
    public HitboxAttackBehaviour(AtaqueInfo ataqueInfo, IAtacador atacador) : base(ataqueInfo, atacador) { }

    public abstract GameObject GetHitbox();
}
