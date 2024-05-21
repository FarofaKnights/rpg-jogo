using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour {
    public string gameSceneName = "Jogo";

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
    }

    public void StartGame(){
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
