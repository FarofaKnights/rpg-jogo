using UnityEngine;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public static HUDController HUD;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        HUD = GetComponentInChildren<HUDController>();
    }
}
