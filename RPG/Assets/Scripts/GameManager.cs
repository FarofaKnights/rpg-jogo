using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Defective.JSON;

public enum GameState { NotStarted, Playing, PauseMenu, Dialog, GameOver, CheatMode, Loja }

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

    public string gameOverSceneName { get { return "GameOver"; } }
    public string startSceneName = "Start", firstSceneName = "Tutorial";
    public string firstPointName = "Inicio";

    public Controls controls;
    public SaveSystem save;
    public ItemManager itemManager;

    public System.Action<string> onBeforeSceneChange, onAfterSceneChange;
    public bool IsLoading { get { return isLoading; } }


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
        controls.Player.CheatMode.performed += ctx => {
            Debug.Log("Cheat mode");
            SetState(GameState.CheatMode);
        };

        controls.Cheat.Exit.performed += ctx => {
            UIController.cheat.Sair();
        };

        if (SceneManager.GetActiveScene().name == startSceneName) SetState(GameState.NotStarted);
        else SetState(GameState.Playing);

        if (!save.variables.HasVariable("slot")) save.variables.SetVariable<int>("slot", 0);
    }

    public void GameOver(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;

        if (save.variables.HasVariable("slot")) {
            int slot = save.variables.GetVariable<int>("slot");
            if (save.HasSave(slot)){
                LoadGame(slot);
                return;
            }
        }
        
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
        GameState oldState = _state;
        _state = newState;

        switch (newState) {
            case GameState.PauseMenu:
                Pausar();
                controls.UI.Enable();
                controls.Player.Disable();
                controls.Loja.Disable();
                break;
            case GameState.Playing:
                Despausar();
                controls.UI.Disable();
                controls.Loja.Disable();
                controls.Player.Enable();
                break;
            case GameState.Dialog:
                controls.UI.Enable();
                controls.Player.Disable();
                break;
            case GameState.GameOver:
                GameOver();
                break;
            case GameState.CheatMode:
                controls.Player.Disable();
                controls.Loja.Disable();
                controls.Cheat.Enable();
                UIController.cheat.Entrar(oldState);
                break;
            case GameState.Loja:
                controls.Player.Disable();
                controls.Loja.Enable();
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

    public void LoadNewGame(int slot = 0) {
        StartCoroutine(LoadNewGameAsync(slot));
    }

    public IEnumerator LoadNewGameAsync(int slot = 0) {
        save.variables.SetVariable<int>("slot", slot);
        yield return GoToSceneAsync(firstSceneName, firstPointName);
        save.Save();
    }

    public void LoadGameFromMenu(int slot = 0) {
        StartCoroutine(LoadGameFromMenuAsync(slot));
    }

    public IEnumerator LoadGameFromMenuAsync(int slot = 0) {
        save.variables.SetVariable<int>("slot", slot);
        if (save.HasSave(slot)) yield return LoadGameAsync(slot);
    }

    public void GoToScene(string scene, string point = "") {
        StartCoroutine(GoToSceneAsync(scene, point));
    }

    public IEnumerator GoToSceneAsync(string scene, string point = "") {
        string currentSceneName = CurrentSceneName();
        if (onBeforeSceneChange != null) onBeforeSceneChange(currentSceneName);

        JSONObject localData = null;
        if (state != GameState.NotStarted) localData = save.LocalSave();

        var asyncLoadLevel = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone){
            yield return null;
        }

        if (onAfterSceneChange != null) onAfterSceneChange(scene);

        if (!isLoading && localData != null) save.LocalLoad(localData);

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
            SaveLastSpawnpoint(point);
        }
    }

    public void SaveLastSpawnpoint(string point){
        save.variables.SetVariable<string>("lastSpawnpoint", point);
        save.variables.SetVariable<string>("lastScene", CurrentSceneName());
    }

    public string CurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

    public void SaveGame(){
        save.Save();
    }

    public void LoadGame(int slot = 0){
        StartCoroutine(LoadGameAsync(slot));
    }

    bool isLoading = false;

    public IEnumerator LoadGameAsync(int slot = 0){
        isLoading = true;
        if (state != GameState.NotStarted) yield return UIController.instance.FadeInAsync();
        save.Load(slot);
        string lastScene = save.variables.GetVariable<string>("lastScene");
        string lastSpawnpoint = save.variables.GetVariable<string>("lastSpawnpoint");
        yield return GoToSceneAsync(lastScene, lastSpawnpoint);
        save.LoadPlayer(slot);
        isLoading = false;
    }

    public T[] GetObjectsOfType<T>() where T : MonoBehaviour {
        return FindObjectsOfType<T>();
    }
}
