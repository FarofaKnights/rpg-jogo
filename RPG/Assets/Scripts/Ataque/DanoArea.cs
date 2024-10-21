using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanoArea : MonoBehaviour {
    public float dano = 1;
    public float periodicidade = 1;
    public List<string> tagsAlvo = new List<string>();

    struct Alvo {
        public PossuiVida vida;
        public float tempoDecorrido;
    }

    List<Alvo> alvos = new List<Alvo>();

    void FixedUpdate() {
        for (int i = 0; i < alvos.Count; i++) {
            Alvo alvo = alvos[i];
            if (alvo.vida == null) {
                alvos.RemoveAt(i);
                i--;
                continue;
            }

            alvo.tempoDecorrido += Time.fixedDeltaTime;
            if (alvo.tempoDecorrido >= periodicidade) {
                alvo.tempoDecorrido = 0;
                alvo.vida.LevarDano(dano);
            }
            alvos[i] = alvo;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (tagsAlvo.Count == 0 || tagsAlvo.Contains(other.tag)) {
            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) {
                alvos.Add(new Alvo { vida = vida, tempoDecorrido = 0 });
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (tagsAlvo.Count == 0 || tagsAlvo.Contains(other.tag)) {
            for (int i = alvos.Count - 1; i >= 0; i--) {
                if (alvos[i].vida.gameObject == other.gameObject) {
                    alvos.RemoveAt(i);
                }
            }
        }
    }

}
