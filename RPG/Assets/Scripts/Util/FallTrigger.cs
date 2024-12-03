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
            Destroy(other);

            if (contarComoMorte) {
                GameManager.instance.GameOver(customSpawnPoint);
            } else {
                if (customSpawnPoint != null && customSpawnPoint != "") {
                    LevelInfo scene = GameManager.instance.CurrentScene();
                    GameManager.instance.GoToScene(scene, customSpawnPoint);
                } else {
                    string[] respawnInfo = GameManager.instance.GetLastSpawnpoint();
                    string scene = respawnInfo[0];
                    string spawnPoint = respawnInfo[1];
                    
                    LevelInfo level = GameManager.instance.loading.GetLevelInfo(scene);
                    GameManager.instance.GoToScene(level, spawnPoint);
                }
            }
        } else if (other.CompareTag("Enemy") || other.CompareTag("Inimigo")) {
            if (other.GetComponent<Inimigo>() != null) 
                other.GetComponent<Inimigo>().Morrer();
        }
    }

}
