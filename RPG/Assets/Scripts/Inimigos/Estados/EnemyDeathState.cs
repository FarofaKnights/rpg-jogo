using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathState : IEnemyState {
    private Inimigo inimigo;
    float timer, maxTimer;
    Renderer[] renderers;

    public enum Estado {
        Vivo, Animacao, Dissolve, Morto
    }
    public Estado estado = Estado.Vivo;

    public EnemyDeathState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Death";

        if (AudioManager.instance.enemyDeath != null)
            AudioManager.instance.enemyDeath.Play();

        inimigo.SpawnParticle();

        if (inimigo.possuiAnimacaoMorte) {
            estado = Estado.Animacao;
            inimigo.animator.SetTrigger("Death");
            inimigo.animator.SetInteger("DeathID", inimigo.deathID);
        } else {
            estado = Estado.Morto;
            GameObject.Destroy(inimigo.gameObject);
            return;
        }

        renderers = inimigo.GetComponentsInChildren<Renderer>();

        inimigo.vidaText.gameObject.SetActive(false);
        
        maxTimer = inimigo.tempoAteDissolver;
        timer = maxTimer;

        inimigo.animator.SetFloat("Vertical", 0);
        inimigo.animator.SetFloat("Horizontal", 0);


        inimigo.GetComponent<NavMeshAgent>().enabled = false;
        inimigo.controller.enabled = false;
    }

    public void Exit() {
        Debug.Log("Ressucitou?? É um milagre!");
        inimigo.GetComponent<NavMeshAgent>().enabled = true;
        inimigo.controller.enabled = true;
    }

    public void Execute() {
        timer -= Time.deltaTime;

        if (estado == Estado.Animacao && timer <= 0) {
            estado = Estado.Dissolve;
            timer = inimigo.tempoDissolvendo;
        } else if (estado == Estado.Dissolve) {
            if (timer <= 0) {
                GameObject.Destroy(inimigo.gameObject);
                estado = Estado.Morto;
            } else {
                float progress = 1 - timer / maxTimer;
                foreach (Renderer r in renderers) {
                    r.material.SetFloat("_Progress", progress);
                }
            }
        }
    }
}
