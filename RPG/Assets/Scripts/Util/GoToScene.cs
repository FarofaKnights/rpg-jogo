using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class GoToScene : MonoBehaviour {
    public string sceneName;
    public string point;

    public void OnPlayerInteract() {
        GameManager.instance.GoToScene(sceneName, point);
    }
}
