using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour {
    public string[] textos;
    
    public void ShowMessage() {
        Fala[] falas = new Fala[textos.Length];
        for (int i = 0; i < textos.Length; i++) {
            falas[i] = new Fala(textos[i]);
        }

        UIController.dialogo.StartDialogo(falas, () => {
            Destroy(gameObject);
        });
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ShowMessage();
        }
    }
}
