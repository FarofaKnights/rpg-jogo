using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : MonoBehaviour {
    public Projetil projetil;
    public Transform pontaSniper;
    
    Transform hittedTarget;
    bool atirou = false;
    Projetil currentProjetil;
    Vector3 exitVector;

    public void Atirar() {
        Transform target = Player.instance.meio.transform;

        GameObject projetilInstanciado = Instantiate(projetil.gameObject, pontaSniper.position, pontaSniper.rotation);
        currentProjetil = projetilInstanciado.GetComponent<Projetil>();
        currentProjetil.Direcionar(target);

        exitVector = currentProjetil.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(pontaSniper.position, exitVector, out hit)) {
            hittedTarget = hit.transform;
            atirou = true;
        } else {
            Debug.Log("Nao acertou nada");
        }
    }

    void FixedUpdate() {
        if (atirou && currentProjetil != null) {
            Vector3 dirProjetilToTarget = hittedTarget.position - currentProjetil.transform.position;
            float dot = Vector3.Dot(dirProjetilToTarget, exitVector);

            if (dot < 0.5f) {
                Acertou();
            }
        }
    }

    void Acertou() {
        atirou = false;

        if (hittedTarget.GetComponent<PossuiVida>() != null) {
            hittedTarget.GetComponent<PossuiVida>().LevarDano(currentProjetil.dano);
        }

        Destroy(currentProjetil.gameObject);
    }

    public void AlertarPlayer() {
        Debug.Log("Player alertado");
    }
}
