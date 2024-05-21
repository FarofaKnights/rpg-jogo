using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porta : MonoBehaviour {
    public GameObject barreira, informativo;
    public QuestItem chave;
    bool dentro;

    void Start() {
        informativo.SetActive(false);
    }

    void Update() {
        if (dentro && Input.GetKeyDown(KeyCode.F)) {
            Abrir();
        }
    }

    public bool Abrir() {
        IInventario inv = Player.instance.inventario;
        ItemData chave = this.chave.GetComponent<Item>().data;
        if (inv.RemoveItem(chave)) {
            barreira.SetActive(false);
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            dentro = true;
            informativo.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            dentro = false;
            informativo.SetActive(false);
        }
    }
}
