using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState {
    public Player player;
    int comboCount = 0;
    int comboIndex = 0;
    bool comboFull = false;

    AtaqueInstance ataqueInstance;

    public PlayerAttackState(Player player) {
        this.player = player;
    }

    public void Enter() {
        // Debug.Log("call of enter");
        if (player.arma == null) {
            player.stateMachine.SetState(player.moveState);
            return;
        }

        ResetCombo();
        Atacar();
    }

    public void Execute() {
        if (ataqueInstance == null) {
            player.stateMachine.SetState(player.moveState);
            return;
        }

        ataqueInstance.Update();
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0) && !comboFull && ataqueInstance.PodeCancelar()) {
            //if (ataqueInstance == null || ataqueInstance.Cancelar()) {
                comboCount++;
                //Debug.Log("call of combo: " + comboCount + " index: " + comboIndex);
                if (player.arma.ataques.Length <= comboCount + 1) comboFull = true;

                ataqueInstance.Cancelar();
            //} 
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            player.animator.SetTrigger("Cancel");
            player.stateMachine.SetState(player.moveState);

            PlayerMoveState moveState = (PlayerMoveState)player.stateMachine.GetCurrentState();
            moveState.Dash();
        }
    }

    public void Exit() {
        if (player.arma != null)
            player.arma.Resetar();
        
        if (ataqueInstance != null) {
            ataqueInstance.onEnd -= onAttackEnd;
            ataqueInstance.Acabou();
        }

        Transform hitboxParent = player.GetInfo().attackHolder.transform;
        foreach (Transform child in hitboxParent) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ResetCombo() {
        comboCount = 0;
        comboIndex = 0;
        comboFull = false;
    }

    void Atacar() {
        Vector3 direcaoCamera = Camera.main.transform.forward;
        player.transform.forward = new Vector3(direcaoCamera.x, 0, direcaoCamera.z);

        ataqueInstance = player.arma.Atacar();
        ataqueInstance.onEnd += onAttackEnd;
    }

    public void onAttackEnd() {
        //Debug.Log("end > " + comboIndex + " < " + comboCount);
        if (player.arma != null && comboCount > 0 && comboIndex < comboCount) {
            if (ataqueInstance != null) {
                ataqueInstance.onEnd -= onAttackEnd;
            }
            comboIndex++;
            Atacar();
        } else {
            player.stateMachine.SetState(player.moveState);
        }
    }
}
