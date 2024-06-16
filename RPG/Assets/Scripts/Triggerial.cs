using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Triggerial : MonoBehaviour
{
    public GameObject texturial;
    public bool sceneTransition = false;
    public string nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(sceneTransition)
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                texturial.SetActive(true);
            }
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
