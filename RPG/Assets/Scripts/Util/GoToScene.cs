using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScene : MonoBehaviour {
    public LevelInfo level;
    public bool onTrigger;

    void Awake() {
        OnTrigger trigger = GetComponent<OnTrigger>();
        if (trigger != null && onTrigger) {
            trigger.tagFilter = "Player";
            trigger.onTriggerEnter += go => Go();
        }
    }

    public void OnPlayerInteract() {
        Go();
    }

    public void Go() {
        GameManager.instance.GoToScene(level);
    }
}
