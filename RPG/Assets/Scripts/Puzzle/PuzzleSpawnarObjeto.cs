using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawnarObjeto : PuzzleResult
{
    public GameObject prefab;
    public Vector3 spawnpos;
    public override void Action()
    {
        Debug.Log("SpawnarObjeto");
        Instantiate(prefab, spawnpos, Quaternion.identity);
    }
}
