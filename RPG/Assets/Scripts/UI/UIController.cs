using UnityEngine;

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

        HideMenu();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleMenu();

            if (menu.activeSelf) SetTab("Equipamentos");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu();
            if (menu.activeSelf)  SetTab("Configuracoes");
        }
    }

    public void ShowMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        menu.SetActive(true);
        equipamentos.Show();
    }

    public void HideMenu() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        menu.SetActive(false);
        equipamentos.Hide();
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
}
