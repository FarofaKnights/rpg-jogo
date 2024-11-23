using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Roda : MonoBehaviour {
    public enum ChecaVelocidade { NavMeshAgent, CharacterController, Ambos }

    [Tooltip("Escolha como a velocidade será checada, tenha em mente que ambos é mais custoso. É necessário chamar MudarTipoChecagem para atualizar o método.")]
    public ChecaVelocidade checaVelocidade = ChecaVelocidade.NavMeshAgent;

    public Animator animator;
    public CharacterController controller;
    public NavMeshAgent navMeshAgent;
    public string floatName = "Speed";

    public System.Action Atualiza;

    void Awake() {
        if (animator == null) return;

        MudarTipoChecagem(checaVelocidade);
    }

    public void MudarTipoChecagem(ChecaVelocidade novoTipo) {
        checaVelocidade = novoTipo;

        switch (checaVelocidade) {
            case ChecaVelocidade.NavMeshAgent:
                Atualiza = AtualizaNavmeshAgent;
                break;
            case ChecaVelocidade.CharacterController:
                Atualiza = AtualizaCharacterController;
                break;
            case ChecaVelocidade.Ambos:
                Atualiza = AtualizaAmbos;
                break;
        }
    }

    void FixedUpdate() {
        Atualiza?.Invoke();
    }

    void AtualizaNavmeshAgent() {
        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat(floatName, speed);
    }

    void AtualizaCharacterController() {
        float speed = controller.velocity.magnitude;
        animator.SetFloat(floatName, speed);
    }

    void AtualizaAmbos() {
        float speed = navMeshAgent.velocity.magnitude;
        float controllerSpeed = controller.velocity.magnitude;

        if (speed > controllerSpeed) {
            animator.SetFloat(floatName, speed);
        } else {
            animator.SetFloat(floatName, controllerSpeed);
        }
    }
}
