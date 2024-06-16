using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class SavePoint : MonoBehaviour {
    public SpawnPoint spawnPoint;
    public void OnPlayerInteract() {
        string spawnPointName = spawnPoint.pointName;
        GameManager.instance.SaveLastSpawnpoint(spawnPointName);
        GameManager.instance.SaveGame();
        GameManager.instance.LoadGame();
    }
}
