using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : MonoBehaviour {
    public Projetil projetil;
    public Transform pontaSniper;
    public LineRenderer lineRenderer;
    public float dano = 40f;
    public GameObject explosaoPrefab;
    
    bool mirando = false;
    Vector3 exitVector;

    public void Mirar() {
        mirando = true;
    }

    public void PararDeMirar() {
        mirando = false;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = false;
    }

    public void Atirar() {
        PararDeMirar();

        Transform target = Player.instance.meio.transform;
        exitVector = target.position - pontaSniper.position;


        RaycastHit hit;
        if (Physics.Raycast(pontaSniper.position, exitVector, out hit)) {
            Acertou(hit.transform);
        }
    }

    void FixedUpdate() {
        if (mirando) {
            Transform target = Player.instance.meio.transform;
            Vector3 fromPontaToTarget = target.position - pontaSniper.position;

            RaycastHit hit;
            if (Physics.Raycast(pontaSniper.position, fromPontaToTarget, out hit)) {
                lineRenderer.SetPosition(0, pontaSniper.position);
                lineRenderer.SetPosition(1, hit.point);
                lineRenderer.enabled = true;
            }
        }
    }

    void Acertou(Transform hittedTarget) {
        if (hittedTarget == null) return;

        if (hittedTarget.GetComponent<PossuiVida>() != null) {
            hittedTarget.GetComponent<PossuiVida>().LevarDano(dano);
        }

        if (explosaoPrefab != null)
            Instantiate(explosaoPrefab, hittedTarget.position, Quaternion.identity);
    }

    public void AlertarPlayer() {
        Debug.Log("Player alertado");
    }
}
