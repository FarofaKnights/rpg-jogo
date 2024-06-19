using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetarAnimator : MonoBehaviour {
    public string booleanName = "";
    public Animator animator;

    public void SetBoolTrue() {
        animator.SetBool(booleanName, true);
    }

    public void SetBoolFalse() {
        animator.SetBool(booleanName, false);
    }

    public void SetBool(string value) {
        animator.SetBool(booleanName, value == "true" ? true : false);
    }
}
