using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OnTrigger))]
public class FallTrigger : MonoBehaviour {
    public string customSpawnPoint;
    public bool contarComoMorte = true;

    void Start() {
        GetComponent<OnTrigger>().onTriggerEnter += OnTrigger;
    }

    void OnTrigger(GameObject other) {
        if (other.CompareTag("Player")) {
            if (contarComoMorte) {
                GameManager.instance.GameOver(customSpawnPoint);
            } else {
                if (customSpawnPoint != null && customSpawnPoint != "") {
                    string scene = GameManager.instance.CurrentSceneName();
                    GameManager.instance.GoToScene(scene, customSpawnPoint);
                } else {
                    string[] respawnInfo = GameManager.instance.GetLastSpawnpoint();
                    string scene = respawnInfo[0];
                    string spawnPoint = respawnInfo[1];
                    
                    GameManager.instance.GoToScene(scene, spawnPoint);
                }
            }
        } else if (other.CompareTag("Enemy") || other.CompareTag("Inimigo")) {
            other.GetComponent<Inimigo>().Morrer();
        }
    }

}
