using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggaAnimacao : MonoBehaviour {
    int index = 0;
    public Animator[] animators;
    public string trigger;

    public void Triggar() {
        if (index >= animators.Length) {
            index = 0;
        }

        animators[index].SetTrigger(trigger);
        index++;
    }
}
