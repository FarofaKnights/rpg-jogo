using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textutorial : MonoBehaviour
{
    public GameObject nextPainel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace)) 
        { 
            if(nextPainel != null)
            {
                nextPainel.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }
}
