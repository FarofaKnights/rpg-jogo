using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    
    public string gameOverSceneName, startSceneName = "Start";

    public Controls controls;


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        controls = new Controls();
    }

    void Start() {
        controls.Player.Enable();
    }

    public void GameOver(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void GameStart(){
        if (startSceneName == null || startSceneName == ""){
            startSceneName = "Start";
            Debug.LogWarning("Bota o nome da cena de inicio no GameManager fazendo o favor >:(");
        }
        SceneManager.LoadScene(startSceneName);
    }

    public void Pausar(){
        Time.timeScale = 0;
    }

    public void Despausar(){
        Time.timeScale = 1;
    }

    public bool IsPaused(){
        return Time.timeScale == 0;
    }
}
