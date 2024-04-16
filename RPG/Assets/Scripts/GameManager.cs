using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Update() {
        // Check if [E] key is pressed
        if (Input.GetKeyDown(KeyCode.E)) {
            UIController.instance.ToggleInventario();
        }
    }

    // o StartCoroutine e o StopCoroutine de classes não monobehaviour referenciam o GameManager.instance
}
