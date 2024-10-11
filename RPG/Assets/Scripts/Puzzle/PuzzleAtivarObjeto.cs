using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAtivarObjeto : PuzzleResult
{
    public GameObject ativar;
    public override void Action()
    {
        Debug.Log("AtivarObjeto");
        ativar.SetActive(true);
    }
}
