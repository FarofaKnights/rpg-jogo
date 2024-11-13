using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IEnemyState {
    private Inimigo inimigo;
    AtaqueInstance ataqueInstance;

    // Usado em um caso especifico
    float waitBeforeLeaving = 0;

    public EnemyAttackState(Inimigo inimigo) {
        this.inimigo = inimigo;
    }

    public void Enter() {
        inimigo.debug.estado_atual = "Attack";
        waitBeforeLeaving = 0;
        // inimigo.attackSound.Play();
        GameManager.instance.StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        yield return new WaitForSeconds(inimigo.tempoAntesDoAtaque);
        StartAttack();
    }

    void StartAttack() {
        if (inimigo == null) return;

        if (inimigo.GetType() == typeof(InimigoComArma)) {
            InimigoComArma inimigoComArma = (InimigoComArma)inimigo;
            if (inimigoComArma.arma != null) ataqueInstance = inimigoComArma.arma.Atacar();
            else if (inimigoComArma.braco != null) {
                inimigoComArma.braco.Ativar();
                waitBeforeLeaving = inimigoComArma.bracoCooldown;
                return;
            }
        }
        else ataqueInstance = inimigo.ataque.Atacar(inimigo);

        ataqueInstance.onEnd += LeaveState;
        ataqueInstance.onStateChange += (AtaqueInstance.Estado e) => {
            inimigo.debug.attack_debug = e.ToString();
        };

        ataqueInstance.onAttack += () => {
            if (inimigo.attackSound != null) inimigo.attackSound.Play();
            else Debug.LogWarning("Inimigo " + inimigo.name + " não possui um som de ataque");
        };

        ataqueInstance.onRecovery += () => {
            if (inimigo.attackSound != null) inimigo.attackSound.Stop();

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
        if (inimigo.GetComponent<PossuiVida>().Vida > 0)
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

        if (waitBeforeLeaving > 0) {
            waitBeforeLeaving -= Time.deltaTime;
            if (waitBeforeLeaving <= 0) LeaveState();
        }
    }
}
