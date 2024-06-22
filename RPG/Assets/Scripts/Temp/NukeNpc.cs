using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeNpc : MonoBehaviour
{
   public GameObject npc;
    public GameObject NextNpc;
    public void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
                Debug.Log("bazinga");
                Destroy(npc);
            NextNpc.SetActive(true);
        }
            
                

            
    }

}
