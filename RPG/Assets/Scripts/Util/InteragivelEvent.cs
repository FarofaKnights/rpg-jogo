using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteragivelEvent : Interagivel {
    public UnityEvent onInteract;

    protected override void Interagir() {
        onInteract.Invoke();
    }
}
