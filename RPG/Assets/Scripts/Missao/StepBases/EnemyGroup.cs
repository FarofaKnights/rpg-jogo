using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour {
    public string groupID;
    public List<Inimigo> inimigos = new List<Inimigo>();

    public System.Action<int> onQuantityChange;
    public System.Action<Inimigo> onEnemyDeath;
    public System.Action onGroupDeath;

    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    int totalEnemies = 0;
    int aliveEnemies = 0;

    public int TotalEnemies { get { return totalEnemies; } }
    public int AliveEnemies { get { return aliveEnemies; } }

    void Awake() {
        totalEnemies = inimigos.Count;
    }

    void Start() {
        CheckIntegrity();
        aliveEnemies = inimigos.Count;
        if (inimigos.Count == 0) {
            isDead = true;
            onGroupDeath?.Invoke();
        } else {
            foreach (Inimigo inimigo in inimigos) {
                inimigo.OnDestroied += () => OnEnemyDeath(inimigo);
            }
        }
        
        // skip a frame
        // StartCoroutine(SkipFrame());
    }

    IEnumerator SkipFrame() {
        yield return new WaitForSeconds(1f);
        CheckIntegrity();
        onQuantityChange?.Invoke(inimigos.Count);
    }

    void OnEnemyDeath(Inimigo inimigo) {
        inimigos.Remove(inimigo);
        CheckIntegrity();
        aliveEnemies = inimigos.Count;
        onQuantityChange?.Invoke(inimigos.Count);
        onEnemyDeath?.Invoke(inimigo);

        if (inimigos.Count == 0) {
            isDead = true;
            onGroupDeath?.Invoke();
        }
    }

    void CheckIntegrity() {
        for (int i = inimigos.Count - 1; i >= 0; i--) {
            if (inimigos[i] == null) {
                inimigos.RemoveAt(i);
            }
        }
    }

    [ContextMenu("Setar filhos automaticamente")]
    void SetChildren() {
        inimigos.Clear();
        foreach (Transform child in transform) {
            Inimigo inimigo = child.GetComponent<Inimigo>();
            if (inimigo != null) {
                inimigos.Add(inimigo);
            }
        }
    }
}
