using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuStart : MonoBehaviour {
    public string gameSceneName = "Jogo";
    private bool visivel = false;
    public GameObject uiConfig;

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

    public void SetTrueVisivel()
    {
        visivel = true;
        uiConfig.SetActive(visivel);        
    }
    public void SetFalseVisivel()
    {
        visivel = false;
        uiConfig.SetActive(visivel);        
    }
}
