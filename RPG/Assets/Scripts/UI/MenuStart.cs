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

        GameManager.instance.SetState(GameState.NotStarted);
    }

    public void StartGame(int slot = 0){
        GameManager.instance.LoadNewGame(slot);
    }

    public void ContinueGame(int slot = 0){
        GameManager.instance.LoadGameFromMenu(slot);
    }

    public void ShowCreditos(){
        SceneManager.LoadScene("Creditos");
    }

    public void TelaInicial() {
        SceneManager.LoadScene("Start");
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
