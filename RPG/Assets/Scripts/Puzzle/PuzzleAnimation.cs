using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAnimation : PuzzleResult
{
    public Animation anim;
    public override void Action()
    {
        Debug.Log("Animacao");
        anim.Play();
    }
}
