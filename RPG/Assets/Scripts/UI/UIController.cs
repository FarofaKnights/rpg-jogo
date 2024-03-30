using UnityEngine;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static HUDController HUD;
    public static InventarioUI inventarioUI;
    public InventarioUI inventarioUIRef;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        HUD = GetComponentInChildren<HUDController>();
        inventarioUI = inventarioUIRef; // Pode haver mais de um inventarioUI, por enquanto especificamos ele aqui
    }

    public void ShowInventario(bool show) {
        inventarioUI.gameObject.SetActive(show);
    }

    public void ToggleInventario() {
        inventarioUI.gameObject.SetActive(!inventarioUI.gameObject.activeSelf);
    }
}
