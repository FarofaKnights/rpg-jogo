using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPoint : MonoBehaviour {
    public void OnPlayerInteract() {
        GameManager.instance.LoadGame();
    }
}
