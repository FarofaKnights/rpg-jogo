using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour {
    [TextArea(3, 10)]
    public string[] textos;
    bool mostrado = false;
    
    public void ShowMessage() {
        Fala[] falas = new Fala[textos.Length];
        for (int i = 0; i < textos.Length; i++) {
            falas[i] = new Fala(textos[i]);
            falas[i].autoNext = true;
        }

        mostrado = true;

        UIController.dialogo.StartDialogo(falas, () => {
            mostrado = false;

            if (gameObject != null) {
                Destroy(gameObject);
            }
        });
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (mostrado) return;
            ShowMessage();
        }
    }

    void OnDestroy() {
        if (mostrado) {
            UIController.dialogo.RemoveDialogoEndEvent();
        }
    }
}
