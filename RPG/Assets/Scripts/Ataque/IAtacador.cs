using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerMode {
    Trigger, Bool
}

public interface IAtacador {
    void OnAtaqueHit(GameObject hit);
    AtacadorInfo GetInfo();
    void MoveWithAttack(float step, float progress);
}

public class AtacadorInfo {
    public Animator animator;
    public GameObject attackHolder;
    public string attackTriggerName;
    public GameObject gameObject;
    public TriggerMode triggerMode;
}