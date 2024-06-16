using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interagivel : OnTrigger {
    /*
     * Esse script chama OnPlayerInteract quando o jogador interage com o objeto,
     * para receber esse evento, inclua a função OnPlayerInteract em um script
     * que também é componente deste mesmo objeto.
     */

    protected bool dentro;
    public GameObject informativo;

    void Start() {
        if (this.tagFilter == "" || this.tagFilter == null) this.tagFilter = "Player";

        onTriggerEnter = (GameObject other) => {
            dentro = true;
            informativo.SetActive(true);
            GameManager.instance.controls.Player.Interact.performed += CheckDentro;
        };
        onTriggerExit = (GameObject other) => {
            dentro = false;
            informativo.SetActive(false);
        };

        informativo.SetActive(false);
    }

    protected void CheckDentro(InputAction.CallbackContext context) {
        if (dentro) Interagir();
    }

    protected virtual void Interagir() {
        gameObject.SendMessage("OnPlayerInteract");
    }

    void OnDestroy() {
        GameManager.instance.controls.Player.Interact.performed -= CheckDentro;
    }
}
