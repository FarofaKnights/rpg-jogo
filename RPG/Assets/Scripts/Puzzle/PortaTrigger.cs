using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortaTrigger : MonoBehaviour
{

    public PortaCondicional porta;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (Animator anim in porta.animacoes)
            {
                anim.SetTrigger("Abrir");
                AudioManager.instance.doorOpen.Play();
            }
            foreach (Collider col in porta.doorCols)
            {
                col.enabled = false;
            }
            Destroy(porta.gameObject);
        }
    }
}
