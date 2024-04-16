using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAtacador {
    void OnAtaqueHit(GameObject hit);

    Animator GetAnimator();
    GameObject GetAttackHitboxHolder();
    string AttackTriggerName();
}
