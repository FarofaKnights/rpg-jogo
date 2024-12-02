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

    public void Morreu(DamageInfo dano)
    {
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }

    public void OnPortaOpen()
    {
        Morreu(new DamageInfo());
    }
}
