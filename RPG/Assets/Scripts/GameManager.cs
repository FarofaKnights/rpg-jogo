using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    bool mostrando = false;

    public string gameOverSceneName;


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Update() {
        // Check if [E] key is pressed
        if (Input.GetKeyDown(KeyCode.E)) {
            UIController.instance.ToggleInventario();
            if (!mostrando) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
        }
    }

    public void GameOver(){
        SceneManager.LoadScene(gameOverSceneName);
    }

    // o StartCoroutine e o StopCoroutine de classes não monobehaviour referenciam o GameManager.instance
}
