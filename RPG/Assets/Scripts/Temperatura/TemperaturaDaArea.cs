using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperaturaDaArea : MonoBehaviour {
    public float temperatura;

    void Start() {
        TemperaturaController.instance.DefinirArea(GetInstanceID(), temperatura);
    }

    void OnTriggerEnter(Collider other) {
        Sentidor sentidor = other.GetComponent<Sentidor>();
        if (sentidor != null) {
            TemperaturaController.instance.AdicionarSentidor(sentidor, GetInstanceID());
        }
    }

    void OnTriggerExit(Collider other) {
        Sentidor sentidor = other.GetComponent<Sentidor>();
        if (sentidor != null) {
            TemperaturaController.instance.RemoverSentidor(sentidor, GetInstanceID());
        }
    }
}
