using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScene : MonoBehaviour {
    public string sceneName;
    public string point;
    public bool onTrigger;
    
    void Start() {
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
        GameManager.instance.GoToScene(sceneName, point);
    }
}
