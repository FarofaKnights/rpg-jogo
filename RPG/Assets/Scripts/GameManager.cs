using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameState { NotStarted, Playing, PauseMenu, Dialog, GameOver }

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    protected GameState _state;
    public GameState state {
        get { return _state; }
        set {
            if (_state == value) return;
            SetState(value);
        }
    }

    public string gameOverSceneName, startSceneName = "Start";

    public Controls controls;
    public SaveSystem save;
    public ItemManager itemManager;

    public System.Action<string> onBeforeSceneChange, onAfterSceneChange;


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        save = new SaveSystem();
        controls = new Controls();
        itemManager = new ItemManager();

        DontDestroyOnLoad(gameObject);
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

    public void SetState(GameState newState){
        _state = newState;

        switch (newState) {
            case GameState.PauseMenu:
                Pausar();
                controls.UI.Enable();
                controls.Player.Disable();
                break;
            case GameState.Playing:
                Despausar();
                controls.UI.Disable();
                controls.Player.Enable();
                break;
            case GameState.Dialog:
                controls.UI.Enable();
                controls.Player.Disable();
                break;
            case GameState.GameOver:
                GameOver();
                break;
        }
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

    public void GoToScene(string scene, string point = "") {
        StartCoroutine(GoToSceneAsync(scene, point));
    }

    public IEnumerator GoToSceneAsync(string scene, string point = "") {
        string currentSceneName = CurrentSceneName();
        if (onBeforeSceneChange != null) onBeforeSceneChange(currentSceneName);

        var asyncLoadLevel = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone){
            yield return null;
        }

        if (onAfterSceneChange != null) onAfterSceneChange(scene);

        if (!isLoading) save.LoadPlayer();

        if (point != "") {
            TeleportPlayerToPoint(point);
        }
    }

    public IEnumerator RefreshScene(){
        string currentSceneName = CurrentSceneName();
        yield return GoToSceneAsync(currentSceneName);
    }

    public void TeleportPlayerToPoint(string point){
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        SpawnPoint selected = null;
        foreach (SpawnPoint spawnPoint in spawnPoints){
            if (spawnPoint.pointName == point){
                selected = spawnPoint;
                break;
            }
        }

        if (selected != null){
            Player.instance.TeleportTo(selected.transform.position);
        }
    }

    public string CurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

    public void SaveGame(){
        save.Save();
    }

    public void LoadGame(){
        StartCoroutine(LoadGameAsync());
    }

    bool isLoading = false;

    public IEnumerator LoadGameAsync(){
        isLoading = true;
        save.Load();
        yield return RefreshScene();
        save.Load();
        isLoading = false;
    }
}
