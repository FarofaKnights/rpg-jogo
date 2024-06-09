using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    public void OnPlayerInteract() {
        GameManager.instance.SaveGame();
    }
}
