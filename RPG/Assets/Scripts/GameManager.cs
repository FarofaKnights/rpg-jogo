using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Defective.JSON;

public enum GameState { NotStarted, Playing, PauseMenu, Dialog, GameOver, CheatMode, Loja, StatChoice }

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    protected GameState _state;
    protected Stack<GameState> _intermediaryState = new Stack<GameState>();

    public GameState state {
        get { return _state; }
        set {
            if (_state == value) return;
            SetState(value);
        }
    }

    public string gameOverSceneName { get { return "GameOver"; } }
    public string startSceneName = "Start", firstSceneName = "Introducao";
    public string firstPointName = "TutorialStart";
    public string _debugCurrentState = "";

    public Controls controls;
    public SaveSystem save;
    public ItemManager itemManager;

    public System.Action<string> onBeforeSceneChange, onAfterSceneChange;
    public System.Action onSaveLoaded;
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

        transform.SetParent(null);
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

    string customRespawnPoint = "";
    string customRespawnScene = "";

    public void GameOver(string respawnAt = ""){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;

        if (respawnAt != "") {
            customRespawnPoint = respawnAt;
            customRespawnScene = CurrentSceneName();
        }

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

    public void SetIntermediaryState(GameState newState){
        _intermediaryState.Push(_state);
        SetState(newState);
    }

    public void RestoreIntermediaryState(GameState defaultState = GameState.Playing){
        if (_intermediaryState.Count > 0){
            SetState(_intermediaryState.Pop());
        } else {
            SetState(defaultState);
        }
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
                controls.Dialog.Disable();
                break;
            case GameState.Playing:
                Despausar();
                controls.Dialog.Disable();
                controls.UI.Disable();
                controls.Loja.Disable();
                controls.Player.Enable();
                break;
            case GameState.Dialog:
                controls.Dialog.Enable();
                break;
            case GameState.GameOver:
                GameOver();
                break;
            case GameState.CheatMode:
                controls.Player.Disable();
                controls.Loja.Disable();
                controls.Dialog.Disable();
                controls.Cheat.Enable();
                UIController.cheat.Entrar(oldState);
                break;
            case GameState.Loja:
                controls.Player.Disable();
                controls.Loja.Enable();
                break;
            case GameState.StatChoice:
                controls.Player.Disable();
                break;
        }

        _debugCurrentState = newState.ToString();
    }


    public void Pausar(){
        AudioManager.instance.playerFootsteps.Pause();
        Time.timeScale = 0;
    }

    public void Despausar(){
        SettingsManager.instance.SaveValues();
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
        yield return GoToSceneAsync(firstSceneName, firstPointName, false);
        save.Save();
        onSaveLoaded?.Invoke();
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

    public IEnumerator GoToSceneAsync(string scene, string point = "", bool saveGame = true) {
        string currentSceneName = CurrentSceneName();
        if (onBeforeSceneChange != null) onBeforeSceneChange(currentSceneName);

        if (state != GameState.NotStarted) {
            if (point != "") SaveLastSpawnpoint(point, scene);
            if (saveGame) save.Save();
        }   

        var asyncLoadLevel = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone){
            yield return null;
        }

        if (onAfterSceneChange != null) onAfterSceneChange(scene);

        if (!isLoading && saveGame) {
            save.Load();
            onSaveLoaded?.Invoke();
        }

        if (point != "") {
            SaveLastSpawnpoint(point);
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
        SaveLastSpawnpoint(point, CurrentSceneName());
    }

    public void SaveLastSpawnpoint(string point, string scene){
        save.variables.SetVariable<string>("lastSpawnpoint", point);
        save.variables.SetVariable<string>("lastScene", scene);
    }

    public string CurrentSceneName(){
        return SceneManager.GetActiveScene().name;
    }

    public void SaveGame(){
        if (UIController.instance != null) UIController.HUD.PopUpSaveInfo();
        save.Save();
        onSaveLoaded?.Invoke();
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

        if (customRespawnPoint != "") {
            lastSpawnpoint = customRespawnPoint;
            lastScene = customRespawnScene;
            customRespawnPoint = "";
            customRespawnScene = "";
        }

        yield return GoToSceneAsync(lastScene, lastSpawnpoint, false);
        save.LoadPlayer(slot);
        isLoading = false;
    }

    public string[] GetLastSpawnpoint(){
        return new string[] { save.variables.GetVariable<string>("lastScene"), save.variables.GetVariable<string>("lastSpawnpoint") };
    }

    public T[] GetObjectsOfType<T>(bool val) where T : MonoBehaviour {
        return FindObjectsOfType<T>(val);
    }

    public T[] GetObjectsOfType<T>() where T : MonoBehaviour {
        return FindObjectsOfType<T>();
    }

    public IEnumerator SlowdownOnHitCoroutine() {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.05f);
        if (!GameManager.instance.IsPaused()) Time.timeScale = 1;
        else Time.timeScale = 0;
    }

    public void SlowdownOnHit() {
        StartCoroutine(SlowdownOnHitCoroutine());
    }
}
