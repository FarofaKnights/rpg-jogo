using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Projetil : MonoBehaviour, IDanoIndireto {
    public float velocidade = 10;
    public float tempoDeVida = 2;
    public float dano = 1;
    public GameObject explosao;
    DamageInfo danoInfo;


    public string tagAlvo = "Player";
    Cinemachine.CinemachineImpulseSource impulseSource;
    public float impulseForce = 1f;

    bool alreadyHit = false;
    public List<GameObject> ignoreList = new List<GameObject>();
    public List<string> ignoreTags = new List<string>();

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
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
            impulseSource.GenerateImpulse(Camera.main.transform.forward * impulseForce);

        Destroy(gameObject, tempoDeVida);
    }

    void FixedUpdate() {
        transform.position += transform.forward * velocidade * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if (ignoreList.Contains(other.gameObject)) return;
        if (ignoreTags.Contains(other.tag)) return;

        if (alreadyHit) return;
        alreadyHit = true;

        if(other.CompareTag("PuzzleTarget")){
            PuzzleResult [] pt = other.GetComponent<PuzzleTarget>().AcoesDoPuzzle;
            foreach(PuzzleResult pr in pt)
            {
                pr.Action();
            }
        }

        else if (other.CompareTag(tagAlvo)) {
            Inimigo inimigo = other.GetComponent<Inimigo>();
            if (inimigo != null) inimigo.deathID = 3;

            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) vida.LevarDano(danoInfo);
            else {
                vida = other.GetComponentInChildren<PossuiVida>();
                if (vida != null) vida.LevarDano(danoInfo);
            }
        } else if(explosao != null) {
            GameObject instancia = Instantiate(explosao, transform.position, transform.rotation);
            SkillArea skillArea = instancia.GetComponent<SkillArea>();
            if (skillArea != null) {
                skillArea.SetInfoFromOrigem(danoInfo);
            }
        }


        Destroy(gameObject);
    }

    public void Direcionar(Transform alvo) {
        transform.LookAt(alvo);
    }
}
