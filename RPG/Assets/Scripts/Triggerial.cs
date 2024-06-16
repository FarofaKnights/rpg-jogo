using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerial : MonoBehaviour
{
    public GameObject texturial;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            texturial.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
