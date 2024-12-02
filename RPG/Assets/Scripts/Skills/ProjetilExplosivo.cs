using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilExplosivo : MonoBehaviour, IDanoIndireto {
    public float velocidade = 10;
    public float tempoDeVida = 2;
    public float dano = 1;
    DamageInfo danoInfo;
    GameObject origemDano;

    public GameObject Explosao;

    public string tagAlvo = "Player";

    bool alreadyHit = false;

    public List<GameObject> ignoreList = new List<GameObject>();

    public void SetInfoFromOrigem(DamageInfo infoOrigem) {
        danoInfo = new DamageInfo(infoOrigem);
    }

    void Awake() {
        if (danoInfo == null)
            danoInfo = new DamageInfo {
                tipoDeDano = TipoDeDano.Projetil,
                formaDeDano = FormaDeDano.Ativo,
                dano = dano,
                origem = gameObject
            };
    }

    void Start() {
        Destroy(gameObject, tempoDeVida);
    }

    void FixedUpdate() {
        transform.position += transform.forward * velocidade * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other) {

        if (other.CompareTag(tagAlvo)) {
            if (alreadyHit) return;
            alreadyHit = true;

            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) vida.LevarDano(danoInfo);
        } else {
            Instantiate(Explosao, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
