using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Projetil : MonoBehaviour {
    public float velocidade = 10;
    public float tempoDeVida = 2;
    public float dano = 1;

    public string tagAlvo = "Player";
    Cinemachine.CinemachineImpulseSource impulseSource;
    public float impulseForce = 1f;

    bool alreadyHit = false;

    void Start() {
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
            impulseSource.GenerateImpulse(Camera.main.transform.forward * impulseForce);

        Destroy(gameObject, tempoDeVida);
    }

    void Update() {
        transform.position += transform.forward * velocidade * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(tagAlvo)) {
            if (alreadyHit) return;
            alreadyHit = true;
            
            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null) {
                vida.LevarDano(dano);
            }
            Destroy(gameObject);
        }
    }
}
