using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMove : MonoBehaviour
{

    public GameObject player;
    public GameObject inimigo;
    public Transform Parent;
    public Transform newParent;
    public void OnTriggerStay(Collider other)
    {
        Debug.Log("Algo tocou na plataforma");
        if (other.CompareTag("Player"))
        {
            Debug.Log("O player entrou na plataforma");
            other.transform.SetParent(newParent);
        }
        if (other.CompareTag("Inimigo"))
        {
            Debug.Log("O inimigo entrou na plataforma");
            inimigo.transform.SetParent(newParent);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("sairam da plataforma");
        if (other.CompareTag("Player"))
        {
            Debug.Log("O player saiu da plataforma");
            other.transform.SetParent(null);
        }
        if (other.CompareTag("Inimigo"))
        {
            Debug.Log("O inimigo saiu da plataforma");
            inimigo.transform.SetParent(null);
        }
    }
}
