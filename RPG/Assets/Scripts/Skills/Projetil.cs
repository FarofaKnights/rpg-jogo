using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Projetil : MonoBehaviour {
    public float velocidade = 10;
    public float tempoDeVida = 2;
    public float dano = 1;
    public GameObject explosao;


    public string tagAlvo = "Player";
    Cinemachine.CinemachineImpulseSource impulseSource;
    public float impulseForce = 1f;

    bool alreadyHit = false;
    public List<GameObject> ignoreList = new List<GameObject>();
    public List<string> ignoreTags = new List<string>();

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
            Debug.Log("Colisoa");
            other.GetComponent<PuzzleTarget>().AcaoDoPuzzle.Action();
        }

        else if (other.CompareTag(tagAlvo)) {
            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) {
                vida.LevarDano(dano);
            }
        } else if(explosao != null) {
            Instantiate(explosao, transform.position, transform.rotation);
        }


        Destroy(gameObject);
    }

    public void Direcionar(Transform alvo) {
        transform.LookAt(alvo);
    }
}
