using System.Linq;
using UnityEngine;

public class TemperaturaAmbiente : MonoBehaviour {
    public float temperatura;

    void Start() {
        TemperaturaController.instance.DefinirArea(GetInstanceID(), temperatura);

        var sentidores = FindObjectsOfType<MonoBehaviour>().OfType<Sentidor>();
        foreach (Sentidor sentidor in sentidores) {

            TemperaturaController.instance.AdicionarSentidor(sentidor, GetInstanceID());
        }
    }
}
