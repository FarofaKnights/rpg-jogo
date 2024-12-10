using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ChangeLogRelation {
    public string versao;
    public GameObject referenciaObjeto;
}

public class MenuStart : MonoBehaviour {
    public string gameSceneName = "Jogo";
    private bool visivel = false;
    public GameObject uiConfig;

    public GameObject settingsPanel;
    public GameObject jogarPanel;
    public string itchURL = "https://farofa-knights.itch.io/sangue-frio";


    [Header("Changelog")]
    public Transform changeLogPanel;
    public ScrollRect changeLogScrollRect;
    public GameObject selectChangeLogButtonPrefab;
    public Transform changeLogSelectHolder;
    public ChangeLogRelation[] changeLogItems;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;

        GameManager.instance.SetState(GameState.NotStarted);
    }

    public void OpenItch() {
        Application.OpenURL(itchURL);
    }

    public void ShowPlayOptions() {
        if (GameManager.instance.save.HasSave(0)) {
            jogarPanel.SetActive(true);
        } else {
            StartGame();
        }
    }

    public void ClosePlayOptions() {
        jogarPanel.SetActive(false);
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

    public void OpenSettings() {
        settingsPanel.SetActive(true);
    }
    public void CloseSettings() {
        SettingsManager.instance.SaveValues();
        settingsPanel.SetActive(false);
    }

    // ChangeLog

    public void ShowChangeLog() {
        LoadChangeLog();
        changeLogPanel.gameObject.SetActive(true);
    }

    public void CloseChangeLog() {
        changeLogPanel.gameObject.SetActive(false);
    }

    public void LoadChangeLog() {
        foreach (Transform child in changeLogSelectHolder) {
            Destroy(child.gameObject);
        }

        foreach (ChangeLogRelation item in changeLogItems) {
            GameObject newButton = Instantiate(selectChangeLogButtonPrefab, changeLogSelectHolder);
            newButton.GetComponentInChildren<Text>().text = item.versao;
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowChangeLogItem(item.versao));
        }
    }

    public void ShowChangeLogItem(string versao) {
        foreach (ChangeLogRelation item in changeLogItems) {
            if (item.versao == versao) {
                changeLogScrollRect.content = item.referenciaObjeto.GetComponent<RectTransform>();
                item.referenciaObjeto.SetActive(true);
            } else {
                item.referenciaObjeto.SetActive(false);
            }
        }

        StartCoroutine(RefreshChangeLog());
    }

    IEnumerator RefreshChangeLog() {
        yield return new WaitForEndOfFrame();
        changeLogScrollRect.verticalNormalizedPosition = 1;
        LayoutRebuilder.ForceRebuildLayoutImmediate(changeLogScrollRect.GetComponent<RectTransform>());
    }
}
