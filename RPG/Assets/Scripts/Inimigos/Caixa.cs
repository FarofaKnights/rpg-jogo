using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caixa : MonoBehaviour
{
    public GameObject dropItem;


    void Start()
    { 

       PossuiVida HP = GetComponent<PossuiVida>();

        HP.onDeath += Morreu;

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
