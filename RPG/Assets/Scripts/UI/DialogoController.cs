using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogoController : MonoBehaviour {
    public Text texto;
    public System.Action OnDialogoEnd;

    Fala[] falas;
    int index = 0;

    void Start() {
        texto.text = "";
    }

    public void StartDialogo(Fala[] falas, System.Action OnDialogoEnd = null) {
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
        texto.text = fala.text;

        Acao acao = fala.acao.GetAcao();
        if (acao != null) {
            acao.Realizar();
        }

        if (fala.autoNext) {
            ShowFala();
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
