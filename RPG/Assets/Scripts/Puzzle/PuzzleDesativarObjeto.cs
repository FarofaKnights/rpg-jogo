using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDesativarObjeto : PuzzleResult
{
    public GameObject desativar;
    public override void Action()
    {
        Debug.Log("DesativarObjeto");
        desativar.SetActive(false);
    }
}
