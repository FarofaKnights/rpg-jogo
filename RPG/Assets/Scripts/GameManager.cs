using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    void Update() {
        // Check if [E] key is pressed
        if (Input.GetKeyDown(KeyCode.E)) {
            UIController.instance.ToggleInventario();
        }
    }
}
