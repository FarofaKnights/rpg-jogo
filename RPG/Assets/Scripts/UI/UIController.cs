using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public interface UITab {
    void Show();
    void Hide();
}

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static HUDController HUD;
    public static AreaEquipamentosUI equipamentos;
    public static ConfiguracoesUI configuracoes;
    public static SistemaUI sistema;
    public static MissaoUI missao;
    public static DialogoController dialogo;
    public static StatChoiceController statChoice;
    public static CheatController cheat;
    public static ModalController modal;
    public static VideoController video;
    public GameObject menu;

    [Header("Black Screen")]
    public GameObject blackScreen;
    public float fadeSpeed = 1f;

    [Header("Tabs")]
    public int selectedTabSize = 22;
    public Color selectedTabColor;
    public int unselectedTabSize = 14;
    public Color unselectedTabColor;
    public Text equipamentosTab, configuracoesTab, sistemaTab, missaoTab;

    #if UNITY_EDITOR
        public bool updateTabsOnValidate = false;
    #endif

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        HUD = GetComponentInChildren<HUDController>(true);
        equipamentos = GetComponentInChildren<AreaEquipamentosUI>(true);
        configuracoes = GetComponentInChildren<ConfiguracoesUI>(true);
        sistema = GetComponentInChildren<SistemaUI>(true);
        missao = GetComponentInChildren<MissaoUI>(true);
        dialogo = GetComponentInChildren<DialogoController>(true);
        statChoice = GetComponentInChildren<StatChoiceController>(true);
        cheat = GetComponentInChildren<CheatController>(true);
        modal = GetComponentInChildren<ModalController>(true);
        video = GetComponentInChildren<VideoController>(true);
    }

    void Start() {
        GameManager.instance.controls.Player.Pause.performed += ShowConfiguracoes;
        GameManager.instance.controls.Player.Itens.performed += ShowEquipamentos;

        GameManager.instance.controls.Loja.Pause.performed += ShowConfiguracoes;
        GameManager.instance.controls.Loja.Itens.performed += ShowEquipamentos;

        GameManager.instance.controls.Dialog.Pause.performed += ShowConfiguracoes;
        GameManager.instance.controls.Dialog.Itens.performed += ShowEquipamentos;

        HideMenu();

        if (GameManager.instance.IsLoading) {
            blackScreen.SetActive(true);
            FadeOut();
        }
    }

    public void FadeIn(float time = -1){
        StartCoroutine(BlackScreenFade(true, time));
    }

    public IEnumerator FadeInAsync(float time = -1){
        yield return StartCoroutine(BlackScreenFade(true, time));
    }

    public void FadeOut(float time = -1){
        StartCoroutine(BlackScreenFade(false, time));
    }

    public IEnumerator FadeOutAsync(float time = -1){
        yield return StartCoroutine(BlackScreenFade(false, time));
    }

    IEnumerator BlackScreenFade(bool isFadeIn = true, float time = -1) {
        float currentAlpha = isFadeIn ? 0 : 1;
        float targetAlpha = isFadeIn ? 1 : 0;
        Image blackScreenImage = blackScreen.GetComponent<Image>();

        float speed = time > 0 ? 1 / time : fadeSpeed;

        blackScreen.SetActive(true);
    
        while (currentAlpha != targetAlpha) {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, speed * Time.deltaTime);
            blackScreenImage.color = new Color(0, 0, 0, currentAlpha);
            yield return null;
        }

        blackScreen.SetActive(isFadeIn);
    }
    
    public void ShowConfiguracoes(InputAction.CallbackContext ctx) {
        ShowMenu();
        SetTab("Configuracoes");
    }

    public void ShowEquipamentos(InputAction.CallbackContext ctx) {
        ShowMenu();
        SetTab("Equipamentos");
    }

    public void ShowMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.SetIntermediaryState(GameState.PauseMenu);

        menu.SetActive(true);
        equipamentos.Show();

        GameManager.instance.controls.UI.Exit.performed += HideMenu;
    }

    public void HideMenu(InputAction.CallbackContext ctx) {
        HideMenu();
    }

    public void HideMenu() {
        GameManager.instance.controls.UI.Exit.performed -= HideMenu;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        menu.SetActive(false);
        equipamentos.Hide();
        configuracoes.Hide();
        sistema.Hide();

        GameManager.instance.RestoreIntermediaryState();
    }

    public void ToggleMenu() {
        if (menu.activeSelf) HideMenu();
        else ShowMenu();
    }

    public void SetTab(string nome) {
        Text tab = null;

        switch (nome) {
            case "Equipamentos":
                equipamentos.Show();
                configuracoes.Hide();
                sistema.Hide();
                missao.Hide();
                tab = equipamentosTab;
                break;
            case "Configuracoes":
                equipamentos.Hide();
                configuracoes.Show();
                sistema.Hide();
                missao.Hide();
                tab = configuracoesTab;
                break;
            case "Sistema":
                equipamentos.Hide();
                configuracoes.Hide();
                sistema.Show();
                missao.Hide();
                tab = sistemaTab;
                break;
            case "Missao":
                equipamentos.Hide();
                configuracoes.Hide();
                sistema.Hide();
                missao.Show();
                tab = missaoTab;
                break;
        }

        Text[] tabs = {equipamentosTab, configuracoesTab, sistemaTab, missaoTab};
        foreach (Text t in tabs) {
            if (t == tab) {
                t.fontSize = selectedTabSize;
                t.color = selectedTabColor;
            } else {
                t.fontSize = unselectedTabSize;
                t.color = unselectedTabColor;
            }
        }
    }

    public void CarregarJogo() {
        HideMenu();
        GameManager.instance.LoadGame();
    }

    void OnDestroy() {
        GameManager.instance.controls.Player.Pause.performed -= ShowConfiguracoes;
        GameManager.instance.controls.Loja.Pause.performed -= ShowConfiguracoes;
        GameManager.instance.controls.Loja.Itens.performed -= ShowEquipamentos;
        GameManager.instance.controls.Player.Itens.performed -= ShowEquipamentos;

        GameManager.instance.controls.Dialog.Pause.performed -= ShowConfiguracoes;
        GameManager.instance.controls.Dialog.Itens.performed -= ShowEquipamentos;
    }

    #if UNITY_EDITOR
    void OnValidate() {
        if (!updateTabsOnValidate) return;
        equipamentos = GetComponentInChildren<AreaEquipamentosUI>(true);
        configuracoes = GetComponentInChildren<ConfiguracoesUI>(true);
        sistema = GetComponentInChildren<SistemaUI>(true);

        SetTab("Configuracoes");
    }
    #endif
}
