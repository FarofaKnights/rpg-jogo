using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : IEnemyState {
    private Inimigo inimigo;
    private NavMeshAgent navMeshAgent;
    
    public enum State { Walking, Waiting }
    public State estado = State.Walking;
    float antigoStoppingDistance, antigaVelocidade;

    float tempoEspera = 0;

    public int pontoAtual = 0;
    int direcaoPatrulha = 1;
    float velocidadePadrao;

    public EnemyPatrolState(Inimigo inimigo) {
        this.inimigo = inimigo;
        navMeshAgent = inimigo.GetComponent<NavMeshAgent>();
    }

    public Transform GetClosestPoint(out int index) {
        float closestDist = float.MaxValue;
        index = 0;

        for (int i = 0; i < inimigo.pontosPatrulha.Count; i++) {
            float dist = GetDistance(inimigo.pontosPatrulha[i]);
            if (dist < closestDist) {
                index = i;
                closestDist = dist;
            }
        }
        
        return inimigo.pontosPatrulha[index];
    }
/*
    public Transform GetClosestPoint() {
        int temp;
        return GetClosestPoint(out temp);
    }
*/
    public float GetDistance(Transform point) {
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh) return float.MaxValue;
        navMeshAgent.SetDestination(point.position);
        if (navMeshAgent.pathPending) return float.MaxValue;
        return navMeshAgent.remainingDistance;
    }

    public int GetNextPoint() {
        int pontoAtual = this.pontoAtual;

        if (inimigo.pontosPatrulha.Count < 2) {
            return 0;
        }

        switch (inimigo.tipoPatrulha) {
            case Inimigo.TipoPatrulha.Ciclica:
                pontoAtual++;
                if (pontoAtual >= inimigo.pontosPatrulha.Count)
                    pontoAtual = 0;
                break;
            case Inimigo.TipoPatrulha.PingPong:
                if (pontoAtual == 0 && direcaoPatrulha == -1) direcaoPatrulha = 1;
                else if (pontoAtual == inimigo.pontosPatrulha.Count - 1) direcaoPatrulha = -1;
                pontoAtual += direcaoPatrulha;
                break;
            case Inimigo.TipoPatrulha.Aleatoria:
                int[] opcoes = new int[inimigo.pontosPatrulha.Count-1];
                for (int i = 0, j = 0; i < inimigo.pontosPatrulha.Count; i++) {
                    if (i == pontoAtual) continue;
                    opcoes[j] = i;
                    j++;
                }

                int auxIndex = Random.Range(0, opcoes.Length);
                pontoAtual = opcoes[auxIndex];
                break;
        }

        return pontoAtual;
    }

    public void Enter() {

        Transform closest = GetClosestPoint(out pontoAtual);
        if (closest.gameObject == null) {
            inimigo.stateMachine.SetState(inimigo.idleState);
            return;
        }

        antigoStoppingDistance = navMeshAgent.stoppingDistance;
        navMeshAgent.stoppingDistance = inimigo.distanciaMinimaPatrulha + 0.1f;
        antigaVelocidade = navMeshAgent.speed;
        navMeshAgent.speed = inimigo.velocidadePatrulha;

        navMeshAgent.enabled = true;
        SetPoint(pontoAtual);

        inimigo.debug.estado_atual = "Patrol - Walking";
    }

    public void SetPoint(int index) {
        pontoAtual = index;
        Transform ponto = inimigo.pontosPatrulha[pontoAtual];
        GameObject target = ponto.gameObject;
        navMeshAgent.SetDestination(target.transform.position);
        inimigo.target = target;
    }

    public void Exit() {
        inimigo.debug.walk_debug = "";
        navMeshAgent.stoppingDistance = antigoStoppingDistance;
        navMeshAgent.speed = antigaVelocidade;
        navMeshAgent.enabled = false;
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Execute() {
        switch(estado) {
            case State.Walking:
                Walking();
                break;
            case State.Waiting:
                Waiting();
                break;
        }

        inimigo.CheckForPlayer();
    }

    public void Walking() {
        Vector3 navMeshDest = inimigo.target.transform.position;

        float dist = Vector3.Distance(inimigo.transform.position, inimigo.target.transform.position);
        float yDist = Mathf.Abs(inimigo.transform.position.y - inimigo.target.transform.position.y);
        inimigo.debug.distancia_alvo = dist;

        if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh) {
            navMeshAgent.SetDestination(navMeshDest);
        }

        Vector3 velocity = navMeshAgent.velocity.normalized;
        inimigo.animator.SetFloat("Vertical", velocity.z);
        inimigo.animator.SetFloat("Horizontal", velocity.x);

        if (dist <= navMeshAgent.stoppingDistance) {
            inimigo.animator.SetFloat("Vertical", 0);
            inimigo.animator.SetFloat("Horizontal", 0);
            estado = State.Waiting;
            inimigo.debug.estado_atual = "Patrol - Waiting";
            tempoEspera = 0;
        }
    }

    public void Waiting() {
        tempoEspera += Time.deltaTime;
        if (tempoEspera >= inimigo.tempoEsperaPatrulha) {
            estado = State.Walking;
            inimigo.debug.estado_atual = "Patrol - Walking";
            pontoAtual = GetNextPoint();
            SetPoint(pontoAtual);
        }
    }
}
