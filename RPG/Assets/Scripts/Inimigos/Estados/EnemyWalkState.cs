using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkState : IEnemyState {
    private Inimigo inimigo;
    private NavMeshAgent navMeshAgent;

    public EnemyWalkState(Inimigo inimigo) {
        this.inimigo = inimigo;
        navMeshAgent = inimigo.GetComponent<NavMeshAgent>();
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Walk";
        GameObject target = inimigo.target;
        if(inimigo.detectado == false)
        {
            inimigo.AlertSound.Play();
            inimigo.detectado = true;
        }

        if (target == null) {
            inimigo.stateMachine.SetState(inimigo.idleState);
            return;
        }

        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(target.transform.position);
    }

    public void Exit() {
        inimigo.debug.walk_debug = "";
        navMeshAgent.enabled = false;
        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);
    }

    public void Execute() {
        Vector3 navMeshDest = inimigo.target.transform.position;
        bool pararDeRodar = false;

        float dist = Vector3.Distance(inimigo.transform.position, inimigo.target.transform.position);
        inimigo.debug.distancia_alvo = dist;

        if (dist > inimigo.rangePerderTarget) { // Caso o target esteja muito longe, ele perde o target
            inimigo.target = null;
            inimigo.stateMachine.SetState(inimigo.idleState);
        } else if (dist < inimigo.maxRangeProximidade) { // Caso o target esteja muito perto, tenta se distanciar (ainda olhando pro target)
            inimigo.debug.walk_debug = "Distanciando";


            // Essa parte do código calcula um ponto, no range de proximidade ideal, que ele deve ir
            Vector3 newPos = inimigo.transform.position;
            Vector3 inimigoToTarget = (inimigo.target.transform.position - inimigo.transform.position).normalized;
            newPos = newPos - inimigoToTarget * inimigo.minRangeProximidade;

            navMeshDest = newPos;
            pararDeRodar = true;
        } else if (inimigo.precisaDeVisaoDireta) { // Caso precise de visão direta, ele verifica se está olhando pro target
            inimigo.debug.walk_debug = "Olho: " + DirectlyLooking() + " Dist: " + navMeshAgent.remainingDistance;

            float remainingDistance = navMeshAgent.remainingDistance;

            if (remainingDistance <= inimigo.minRangeProximidade && DirectlyLooking()) { // Se na distancia correta e olhando pro target, ele ataca
                inimigo.stateMachine.SetState(inimigo.attackState);
            } else if (remainingDistance <= inimigo.minRangeProximidade * 1.2f) { // Se proximo da distancia correto, tenta olhar pro target

                // Calcula a direção que ele deve olhar e interpola a rotação atual para essa direção
                Vector3 lookPos = inimigo.target.transform.position - inimigo.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, 10.0f * Time.fixedDeltaTime);

                if (remainingDistance <= inimigo.minRangeProximidade) { // E ainda, nessa de girar, checa se está na distancia correta
                    navMeshDest = inimigo.transform.position; // Caso esteja, não precisa andar
                }

            }
        } else { // Caso não precise de visão direta, ele só checa a distancia para atacar
            inimigo.debug.walk_debug = "Sem olhar e andando";
            if (navMeshAgent.remainingDistance <= inimigo.minRangeProximidade) {
                inimigo.stateMachine.SetState(inimigo.attackState);
            }
        }

        if (navMeshAgent.enabled) {
            navMeshAgent.updateRotation = !pararDeRodar;
            navMeshAgent.SetDestination(navMeshDest);
        }

        Vector3 velocity = navMeshAgent.velocity.normalized;
        inimigo.animator.SetFloat("Vertical", velocity.z);
        inimigo.animator.SetFloat("Horizontal", velocity.x);
    }

    bool DirectlyLooking() {
        // Basicamente, compara o forward do inimigo com a direção do target para saber se ele está olhando pro target
        Vector3 direction = (inimigo.target.transform.position - inimigo.transform.position).normalized;
        float dotProd = Vector3.Dot(inimigo.transform.forward, direction); // Em suma, o dot product retorna o quão parecidos são os vetores, -1 é opostos, 1 é iguais
        inimigo.debug.dot_prod_olhando = dotProd;

        // Isso aqui é bem engraçado, mas ele nunca chega a 1.0f, então eu coloquei 0.999f para ser preciso
        // E pode parecer que é muita precisão, mas não é, 0.99f tem muita chance dele ficar preso errando o ataque.
        // ERRATA: Isso tá certo de fato, mas precisão perfeita tem muita chance de ficar preso.
        // Na maioria das situações é melhor ser impreciso e usar outros métodos para garantir que o inimigo não erre o ataque.
        return dotProd >= inimigo.precisaoDaVisao;
    }
}
