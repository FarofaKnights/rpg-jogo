using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAnimation : PuzzleResult
{
    public Animator animator;
    public string triggerName;
    public override void Action()
    {
        Debug.Log("Animacao");
        animator.SetTrigger(triggerName);
    }
}
