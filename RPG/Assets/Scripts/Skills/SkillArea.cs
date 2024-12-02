using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillArea : MonoBehaviour, IDanoIndireto {
    float scaleInicial;
    public float scaleFinal = 1;
    float tempoDecorrido = 0;
    public float tempoDeVida = 2;
    public float dano = 1;
    DamageInfo danoInfo;
    public string tagAlvo = "Player";
    
    List<GameObject> jaAtingidos = new List<GameObject>();

    public void SetInfoFromOrigem(DamageInfo infoOrigem) {
        danoInfo = new DamageInfo(infoOrigem);
    }

    void Awake() {
        if (danoInfo == null)
            danoInfo = new DamageInfo {
                tipoDeDano = TipoDeDano.Area,
                formaDeDano = FormaDeDano.Ativo,
                dano = dano,
                origem = gameObject
            };
    }

    void Start() {
        scaleInicial = transform.localScale.x;
        Destroy(gameObject, tempoDeVida);
    }

    void Update() {
        tempoDecorrido += Time.deltaTime;
        float t = tempoDecorrido / tempoDeVida;
        float scale = Mathf.Lerp(scaleInicial, scaleFinal, t);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(tagAlvo)) {
            if (jaAtingidos.Contains(other.gameObject)) return;
            jaAtingidos.Add(other.gameObject);
            
            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) {
                vida.LevarDano(danoInfo);
            }
        }
    }
}
