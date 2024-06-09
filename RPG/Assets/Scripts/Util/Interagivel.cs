using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interagivel : OnTrigger {
    /*
     * Esse script chama OnPlayerInteract quando o jogador interage com o objeto,
     * para receber esse evento, inclua a função OnPlayerInteract em um script
     * que também é componente deste mesmo objeto.
     */

    protected bool dentro;
    public GameObject informativo;

    void Start() {
        onTriggerEnter = (GameObject other) => {
            dentro = true;
            informativo.SetActive(true);
            GameManager.instance.controls.Player.Interact.performed += ctx => {
                if (dentro) Interagir();
            };
        };
        onTriggerExit = (GameObject other) => {
            dentro = false;
            informativo.SetActive(false);
        };

        informativo.SetActive(false);
    }

    protected virtual void Interagir() {
        gameObject.SendMessage("OnPlayerInteract");
    }
}
