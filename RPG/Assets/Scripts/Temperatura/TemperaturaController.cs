using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface Sentidor {
    void SentirTemperatura(float modTemperatura);
}

public class TemperaturaController : MonoBehaviour {
    public static TemperaturaController instance;

    public Volume volume;

    public class AreaTemperatura {
        public int id;
        public float temperatura;
        public List<Sentidor> sentidores;

        public AreaTemperatura(int id, float temperatura = 0) {
            this.id = id;
            this.temperatura = temperatura;
            sentidores = new List<Sentidor>();
        }
    }

    public List<AreaTemperatura> areas = new List<AreaTemperatura>();
    Dictionary<Sentidor, float> aux_temperaturaAtual = new Dictionary<Sentidor, float>();

    public float cicloPorSegundo = 1f;
    public int divisoesPorCiclo = 10;
    int cicloAtual = 0;
    

    void Awake() {
        instance = this;
    }

    void Start() {
        UpdateFrioVolume(0);
        InvokeRepeating("Ciclo", 0, 1.0f / (divisoesPorCiclo * cicloPorSegundo));
    }

    void Ciclo() {
        cicloAtual++;

        aux_temperaturaAtual.Clear();

        foreach (AreaTemperatura area in areas) {
            if (area == null) continue;
            float temperatura = area.temperatura / divisoesPorCiclo;

            foreach (Sentidor sentidor in area.sentidores) {
                if (!aux_temperaturaAtual.ContainsKey(sentidor)) {
                    aux_temperaturaAtual.Add(sentidor, 0);
                }

                aux_temperaturaAtual[sentidor] += temperatura;
            }
        }

        foreach (KeyValuePair<Sentidor, float> sentidor in aux_temperaturaAtual) {
            sentidor.Key.SentirTemperatura(sentidor.Value);
        }
    }

    public void UpdateFrioVolume(float porcentagem) {
        if (volume == null) return;

        volume.weight = porcentagem;
    }

    AreaTemperatura GetArea(int id) {
        foreach (AreaTemperatura area in areas) {
            if (area.id == id) {
                return area;
            }
        }

        return null;
    }

    public float GetTemperaturaSentidor(Sentidor sentidor) {
        float temperatura = 0;

        foreach (AreaTemperatura area in areas) {
            if (area == null) continue;

            foreach (Sentidor sent in area.sentidores) {
                if (sent == sentidor) {
                    temperatura += area.temperatura;
                }
            }
        }

        return temperatura;
    }

    public void DefinirArea(int id, float temperatura = 0) {
        AreaTemperatura area = GetArea(id);

        if (area == null) {
            area = new AreaTemperatura(id);
            areas.Add(area);
        }

        area.temperatura = temperatura;
    }

    public void RemoverArea(int id) {
        AreaTemperatura area = GetArea(id);
        if (area == null) return;

        for (int i = areas.Count - 1; i >= 0; i--) {
            int idAtual = areas[i].id;
            if (idAtual == id) areas.RemoveAt(i);
        }
    }

    public bool AdicionarSentidor(Sentidor sentidor, int area) {
        AreaTemperatura areaObj = GetArea(area);
        if (areaObj == null) return false;

        if (!areaObj.sentidores.Contains(sentidor)) {
            areaObj.sentidores.Add(sentidor);
        }

        return true;
    }

    public void RemoverSentidor(Sentidor sentidor) {
        foreach (AreaTemperatura area in areas) {
            if (area == null) continue;

            for (int i = area.sentidores.Count - 1; i >= 0; i--) {
                Sentidor sent = area.sentidores[i];
                if (sent == sentidor) area.sentidores.RemoveAt(i);
            }
        }
    }

    public bool RemoverSentidor(Sentidor sentidor, int area) {
        AreaTemperatura areaObj = GetArea(area);
        if (areaObj == null) return false;

        for (int i = areaObj.sentidores.Count - 1; i >= 0; i--) {
            Sentidor sent = areaObj.sentidores[i];
            if (sent == sentidor) {
                areaObj.sentidores.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
