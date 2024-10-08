using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogoController : MonoBehaviour {
    public Text texto;
    public System.Action OnDialogoEnd;
    public float autoNextDelay = 5f;

    Fala[] falas;
    int index = 0;

    bool playerLocked = false;


    Fala onHoldToAutoNext, currentFala;

    void Start() {
        texto.text = "";
    }

    public void StartDialogo(Dialogo dialogo, System.Action OnDialogoEnd = null, bool lockPlayer = true) {
        StartDialogo(dialogo.falas.ToArray(), OnDialogoEnd, lockPlayer);
    }

    public void StartDialogo(Fala[] falas, System.Action OnDialogoEnd = null, bool lockPlayer = true) {
        if (this.falas != null) {
            HandleDialogoEnd();
            StopAllCoroutines();
        }

        this.falas = falas;
        index = 0;

        playerLocked = lockPlayer;

        if (lockPlayer) {
            GameManager.instance.controls.Dialog.Interact.performed += Proximo;
            GameManager.instance.SetIntermediaryState(GameState.Dialog);
        } else {
            GameManager.instance.controls.UI.Interact.performed += Proximo;
        }

        this.OnDialogoEnd += OnDialogoEnd;

        ShowFala();
    }

    void ShowFala() {
        if (falas == null) return;
        if (index >= falas.Length) {
            HandleDialogoEnd();
            return;
        }

        Fala fala = falas[index];
        ProcessarFala(fala);
        index++;
    }

    void ProcessarFala(Fala fala) {
        currentFala = fala;
        texto.text = fala.text;

        if (fala.acao != null) {
            Acao acao = fala.acao.GetAcao();
            if (acao != null) {
                acao.Realizar();
            }
        }

        if (fala.autoNext) {
            onHoldToAutoNext = fala;
            StartCoroutine(AutoNext());
        }
    }

    IEnumerator AutoNext() {
        yield return new WaitForSeconds(autoNextDelay);
        if (currentFala == onHoldToAutoNext) {
            Proximo();
        }
    }

    void Proximo() {
        ShowFala();
    }

    void Proximo(InputAction.CallbackContext ctx) {
        Proximo();
    }

    void HandleDialogoEnd() {
        falas = null;
        index = 0;

        texto.text = "";

        if (playerLocked) {
            GameManager.instance.controls.Dialog.Interact.performed -= Proximo;
            GameManager.instance.RestoreIntermediaryState();
        } else {
            GameManager.instance.controls.UI.Interact.performed -= Proximo;
        }

        System.Action end = OnDialogoEnd;
        OnDialogoEnd = null;
        end?.Invoke();
    }

    public void RemoveDialogoEndEvent() {
        OnDialogoEnd = null;
    }
}
