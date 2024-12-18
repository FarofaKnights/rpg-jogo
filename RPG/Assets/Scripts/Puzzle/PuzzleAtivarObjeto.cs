using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAtivarObjeto : PuzzleResult
{
    public GameObject ativar;
    public override void Action()
    {
        Debug.Log("AtivarObjeto");

        OcclusionPortal occlusion = ativar.GetComponent<OcclusionPortal>();
        if (occlusion != null) occlusion.open = false;

        ativar.SetActive(true);
    }
}
