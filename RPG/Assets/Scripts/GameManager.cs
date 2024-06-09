using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    
    public string gameOverSceneName, startSceneName = "Start";

    public Controls controls;

    public SaveSystem save;

    public System.Action<string> onBeforeSceneChange, onAfterSceneChange;


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        save = new SaveSystem();
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

    public void GoToScene(string scene, string point){
        string currentSceneName = CurrentSceneName();
        if (onBeforeSceneChange != null) onBeforeSceneChange(currentSceneName);
        SceneManager.LoadScene(scene);
        if (onAfterSceneChange != null) onAfterSceneChange(scene);
        
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        SpawnPoint selected = null;
        foreach (SpawnPoint spawnPoint in spawnPoints){
            if (spawnPoint.pointName == point){
                selected = spawnPoint;
                break;
            }
        }

        if (selected != null){
            Player.instance.transform.position = selected.transform.position;
        }
    }

    public string CurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

}
