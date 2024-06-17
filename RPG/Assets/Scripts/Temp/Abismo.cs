using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Abismo : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Coisa");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Playyeyey");
            Destroy(other.gameObject);
            GameManager.instance.GameOver();
        }
       
    }
}
