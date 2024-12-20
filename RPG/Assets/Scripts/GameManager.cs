using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.Video;
using Defective.JSON;

public enum GameState { NotStarted, Playing, PauseMenu, Dialog, GameOver, CheatMode, Loja, StatChoice, Cutscene }

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

    public string startSceneName = "Start", firstSceneName = "Introducao";
    public string endSceneName = "End";
    public LevelInfo firstSceneInfo;
    public string firstPointName = "TutorialStart";
    public string _debugCurrentState = "";
    public LoadingUI loadingUI;

    bool inimigosAtivos = true;

    public Controls controls;
    public SaveSystem save;
    public ItemManager itemManager;
    public LoadingController loading;

    public System.Action<string> onBeforeSceneChange, onAfterSceneChange;
    public System.Action onSaveLoaded;
    public bool IsLoading { get { return isLoading; } }

    public ItemData pecasItem;
    public bool isInMenuShowingCreditos = false;
    bool isLoadFromMenu = false;


    [HideInInspector] public CacheLoader<AudioClip> loaded_audioClips = new CacheLoader<AudioClip>("Audio");
    [HideInInspector] public CacheLoader<VideoClip> loaded_videoClips = new CacheLoader<VideoClip>("Video");


    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        save = new SaveSystem();
        controls = new Controls();
        itemManager = new ItemManager();
        loading = new LoadingController();

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
            } else {
                GoToScene(firstSceneInfo, "");
            }
        } else {
            save.variables.SetVariable<int>("slot", 0);
            GoToScene(firstSceneInfo, "");
        }
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
                controls.Player.Disable();
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
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                controls.Player.Disable();
                controls.Loja.Enable();
                break;
            case GameState.StatChoice:
                controls.Player.Disable();
                controls.Loja.Disable();
                controls.Dialog.Disable();
                controls.UI.Disable();
                break;
            case GameState.Cutscene:
                controls.Player.Disable();
                controls.Loja.Disable();
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
        save.variables.Clear();
        save.variables.SetVariable<int>("slot", slot);
        yield return GoToSceneAsync(firstSceneInfo, "", false);
        save.Save();
        onSaveLoaded?.Invoke();
    }

    public void LoadGameFromMenu(int slot = 0) {
        save.variables.SetVariable<int>("slot", slot);
        if (save.HasSave(slot)) {
            isLoadFromMenu = true;
            LoadGame(slot);
        }
    }

    public void GoToScene(LevelInfo level, string customPoint = "") {
        StartCoroutine(GoToSceneAsync(level, customPoint));
    }

    void LoadSaveAfterGoToSceneAsync() {
        save.Load();
        onSaveLoaded?.Invoke();
    }

    void TeleportPlayerToPointAfterGoToSceneAsync(string point) {
        SaveLastSpawnpoint(point);
        TeleportPlayerToPoint(point);
    }

    public IEnumerator GoToSceneAsync(LevelInfo level, string customPoint = "", bool saveGame = true, List<System.Action> onBeforeEndLoad = null) {
        string currentSceneName = CurrentSceneName();
        if (onBeforeSceneChange != null) onBeforeSceneChange(currentSceneName);

        if (state != GameState.NotStarted) {
            if (level.pontoInicial != "") SaveLastSpawnpoint(level.pontoInicial, level.nomeCena);
            if (saveGame) save.Save();
        }

        List<System.Action> onLoaded = new List<System.Action>();

        if (onAfterSceneChange != null) onLoaded.Add(() => onAfterSceneChange(level.nomeCena));

        if (!isLoading && saveGame) {
            onLoaded.Add(LoadSaveAfterGoToSceneAsync);
        }

        string ponto = customPoint != "" ? customPoint : level.pontoInicial;

        if (ponto != "") {
            onLoaded.Add(() => TeleportPlayerToPointAfterGoToSceneAsync(ponto));
        }

        if (onBeforeEndLoad != null) onLoaded.AddRange(onBeforeEndLoad);

        yield return StartCoroutine(loading.LoadSceneAsync(level, onLoaded.ToArray()));
        Debug.Log("Scene loaded: " + level.nomeCena);

    }

    public IEnumerator RefreshScene(){
        yield return GoToSceneAsync(CurrentScene());
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

    public LevelInfo CurrentScene() {
        return loading.GetLevelInfo(CurrentSceneName());
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

        LevelInfo level = loading.GetLevelInfo(lastScene);
        yield return GoToSceneAsync(level, lastSpawnpoint, false);
/*
        if (isLoadFromMenu) {
            isLoadFromMenu = false;
            save.Load(slot);
        }
*/
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

    public void SetInimigosAtivos(bool ativos){
        inimigosAtivos = ativos;
        Inimigo[] inimigos = FindObjectsOfType<Inimigo>();
        foreach (Inimigo inimigo in inimigos){
            if (inimigo == null || inimigo.gameObject == null) continue;
            inimigo.PodeSeguir(ativos);
        }
    }

    public void SetCutsceneMode(bool ativo) {
        if (ativo) {
            SetIntermediaryState(GameState.Cutscene);
        } else {
            RestoreIntermediaryState();
        }

        UIController.HUD.gameObject.SetActive(!ativo);
        Player.instance.SetarControle(!ativo);
    }

    public void ReturnToCutscenesScreen() {
        StartCoroutine(ReturnToCutscenesScreenAsync());
    }

    public void OpenCutscenesScreen() {
        MenuStart menuStart = FindObjectOfType<MenuStart>();
        if (menuStart != null) {
            menuStart.OpenExtras();
            menuStart.OpenExtrasCutscenes();
        }
    }

    IEnumerator ReturnToCutscenesScreenAsync() {
        yield return SceneManager.LoadSceneAsync(startSceneName);
        OpenCutscenesScreen();
    }

    public void PlayCutsceneFromExtras(QuestInfo questInfo) {
        StartCoroutine(PlayCutsceneFromExtrasAsync(questInfo));
    }

    IEnumerator PlayCutsceneFromExtrasAsync(QuestInfo questInfo) {
        LevelInfo level = questInfo.level;
        string cutscenePoint = "CutsceneHolder";

        List<System.Action> onLoaded = new List<System.Action>();
        onLoaded.Add(() => {
            QuestManager.instance.LoadQuest(questInfo);
            QuestManager.instance.StartQuest(questInfo.questId);
        });

        yield return GoToSceneAsync(questInfo.level, cutscenePoint, false, onLoaded);
    }
}
