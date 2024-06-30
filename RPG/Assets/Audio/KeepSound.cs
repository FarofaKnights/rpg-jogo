using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepSound : MonoBehaviour
{
    public static KeepSound instance;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if(!CheckScene())
        {
            Destroy(this.gameObject);
        }
    }

    public bool CheckScene()
    {
        if (SceneManager.GetActiveScene().name == "Start" || SceneManager.GetActiveScene().name == "Settings")
        {
            return true;
        }
        return false;
    }
}
