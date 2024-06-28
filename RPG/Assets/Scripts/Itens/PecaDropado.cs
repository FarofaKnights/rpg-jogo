using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PecaDropado : MonoBehaviour {
    public int quantidade = 1;

    void Start() {
        if (quantidade <= 0) {
            Debug.LogWarning("PecaDropado com quantidade menor ou igual a zero, destruindo objeto.");
            Destroy(gameObject);
        }
    }
}
