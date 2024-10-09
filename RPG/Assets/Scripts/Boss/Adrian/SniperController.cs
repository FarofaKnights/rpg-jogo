using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SniperController : MonoBehaviour {
    public Projetil projetil;
    public Transform pontaSniper;
    public LineRenderer lineRenderer;
    public NavMeshAgent agent;
    public CharacterController controller;
    public Animator animator;
    public LayerMask floor;

    enum EstadoPulo { PULANDO, CAINDO, PARADO }
    EstadoPulo estadoPulo = EstadoPulo.PARADO;


    public float dano = 40f;
    public GameObject explosaoPrefab;
    
    bool mirando = false;
    bool movendoController = false;

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
        RaycastHit hit;

        if (mirando) {
            Transform target = Player.instance.meio.transform;
            Vector3 fromPontaToTarget = target.position - pontaSniper.position;

            if (Physics.Raycast(pontaSniper.position, fromPontaToTarget, out hit)) {
                lineRenderer.SetPosition(0, pontaSniper.position);
                lineRenderer.SetPosition(1, hit.point);
                lineRenderer.enabled = true;
            }
        }

        Vector3 directionMove = transform.forward * 2f;
        

        switch (estadoPulo) {
            case EstadoPulo.PULANDO:
                directionMove += transform.up;
                controller.Move(directionMove * Time.fixedDeltaTime);
                break;
            case EstadoPulo.CAINDO:
                if (Physics.Raycast(controller.transform.position, Vector3.down, out hit, 0.01f, floor)) {
                    estadoPulo = EstadoPulo.PARADO;
                    animator.SetBool("Pulando", false);
                    controller.enabled = false;
                } else {
                    directionMove.y -= 9.8f;
                    controller.Move(directionMove * Time.fixedDeltaTime);
                }
                break;
            case EstadoPulo.PARADO:
                break;
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

    public void SetaPulando() {
        controller.enabled = true;
        agent.enabled = false;

        estadoPulo = EstadoPulo.PULANDO;
    }

    public void SetaCaindo() {
        controller.enabled = true;
        agent.enabled = false;

        estadoPulo = EstadoPulo.CAINDO;
    }
}
