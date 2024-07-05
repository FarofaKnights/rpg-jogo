using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caixa : MonoBehaviour
{
    public GameObject dropItem;


    void Start()
    { 
        PossuiVida HP = GetComponent<PossuiVida>();
        
        if (HP != null) HP.onDeath += Morreu;
    }

    public void Morreu()
    {
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }

    public void OnPortaOpen()
    {
        Morreu();
    }
}
