using UnityEngine;
using UnityEngine.InputSystem;

public interface UITab {
    void Show();
    void Hide();
}

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static HUDController HUD;
    public static AreaEquipamentosUI equipamentos;
    public static ConfiguracoesUI configuracoes;
    public static DialogoController dialogo;
    public static CheatController cheat;
    public GameObject menu;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        HUD = GetComponentInChildren<HUDController>(true);
        equipamentos = GetComponentInChildren<AreaEquipamentosUI>(true);
        configuracoes = GetComponentInChildren<ConfiguracoesUI>(true);
        dialogo = GetComponentInChildren<DialogoController>(true);
        cheat = GetComponentInChildren<CheatController>(true);
    }

    void Start() {
        GameManager.instance.controls.Player.Pause.performed += ShowConfiguracoes;
        GameManager.instance.controls.Player.Itens.performed += ShowEquipamentos;

        HideMenu();
    }

    void ShowConfiguracoes(InputAction.CallbackContext ctx) {
        ShowMenu();
        SetTab("Configuracoes");
    }

    void ShowEquipamentos(InputAction.CallbackContext ctx) {
        ShowMenu();
        SetTab("Equipamentos");
    }

    public void ShowMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.SetState(GameState.PauseMenu);

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

        GameManager.instance.SetState(GameState.Playing);
    }

    public void ToggleMenu() {
        if (menu.activeSelf) HideMenu();
        else ShowMenu();
    }

    public void SetTab(string nome) {
        switch (nome) {
            case "Equipamentos":
                equipamentos.Show();
                configuracoes.Hide();
                break;
            case "Configuracoes":
                configuracoes.Show();
                equipamentos.Hide();
                break;
        }
    }

    void OnDestroy() {
        GameManager.instance.controls.Player.Pause.performed -= ShowConfiguracoes;
        GameManager.instance.controls.Player.Itens.performed -= ShowEquipamentos;
    }
}
