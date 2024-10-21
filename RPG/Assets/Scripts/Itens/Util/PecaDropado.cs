using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PecaDropado : Drop {
    public int quantidade = 1;
    Informativo informativo;

    void Start() {
        informativo = GetComponent<Informativo>();

        if (quantidade <= 0) {
            Debug.LogWarning("PecaDropado com quantidade menor ou igual a zero, destruindo objeto.");
            Destroy(gameObject);
        }
    }

    public override void OnCollect() {
        Player.Atributos.pecas.Add(quantidade);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            informativo.informativo.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            informativo.informativo.SetActive(false);
        }
    }
}
