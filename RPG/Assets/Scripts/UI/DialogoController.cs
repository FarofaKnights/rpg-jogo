using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogoController : MonoBehaviour {
    public Text texto;
    public System.Action OnDialogoEnd;
    public float autoNextDelay = 5f;

    Fala[] falas;
    int index = 0;


    Fala onHoldToAutoNext, currentFala;

    void Start() {
        texto.text = "";
    }

    public void StartDialogo(Fala[] falas, System.Action OnDialogoEnd = null) {
        if (this.falas != null) {
            HandleDialogoEnd();
            StopAllCoroutines();
        }

        this.falas = falas;
        index = 0;

        GameManager.instance.controls.UI.Interact.performed += ctx => Proximo();
        GameManager.instance.SetState(GameState.Dialog);

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

    void HandleDialogoEnd() {
        OnDialogoEnd?.Invoke();
        OnDialogoEnd = null;

        falas = null;
        index = 0;

        texto.text = "";

        GameManager.instance.controls.UI.Interact.performed -= ctx => Proximo();
        GameManager.instance.SetState(GameState.Playing);
    }
}
