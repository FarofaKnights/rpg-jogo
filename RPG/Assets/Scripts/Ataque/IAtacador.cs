using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerMode {
    Trigger, Bool
}

public interface IAtacador {
    void OnAtaqueHit(GameObject hit);

    Animator GetAnimator();
    GameObject GetAttackHolder();
    string AttackTriggerName();
    GameObject GetSelf();
    TriggerMode GetTriggerMode();
    void MoveWithAttack(float step, float progress);
}
