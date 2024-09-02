using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IEnemyState {
    private Inimigo inimigo;
    AtaqueInstance ataqueInstance;

    public EnemyAttackState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Attack";
        // inimigo.attackSound.Play();
        GameManager.instance.StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        yield return new WaitForSeconds(0.25f);
        StartAttack();
    }

    void StartAttack() {
        if (inimigo == null) return;

        ataqueInstance = inimigo.ataque.Atacar(inimigo);
        ataqueInstance.onEnd += LeaveState;
        ataqueInstance.onStateChange += (AtaqueInstance.Estado e) => {
            inimigo.debug.attack_debug = e.ToString();
        };

        ataqueInstance.onAttack += () => {
            inimigo.attackSound.Play();
        };

        ataqueInstance.onRecovery += () => {
            inimigo.attackSound.Stop();

            // Condição especial de descanso longo
            if (inimigo.descansoMaiorAposXAtaques > 0) {
                inimigo.ataquesFeitos++;
            
                if (inimigo.ataquesFeitos >= inimigo.descansoMaiorAposXAtaques){
                    ataqueInstance.RecoveryLongo(inimigo.multDescansoMaior);
                    inimigo.ataquesFeitos = 0;
                }
            }
            
        };

    }

    void LeaveState() {
        inimigo.stateMachine.SetState(inimigo.walkState);
    }

    public void Exit() {
        
        // inimigo.attackSound.Stop();
        if (ataqueInstance != null)
            ataqueInstance.onEnd -= LeaveState;
    }

    public void Execute() {
        if (ataqueInstance != null) {
            ataqueInstance.Update();
        }
    }
}
