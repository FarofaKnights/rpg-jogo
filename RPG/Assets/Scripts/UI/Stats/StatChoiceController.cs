using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatChoiceController : MonoBehaviour {
    public StatChoicer vida, forca, calor;
    public Text informaPontos;
    public DisableableButton confirmar;

    public int pontosDisponiveisInicialmente = 0;
    public int pontosRestantes = 0;
    Promise openPromise;

    bool hadMouse = false;

    public GameObject holder;

    void Start() {
        GameManager.instance.onSaveLoaded += OnLoadedGame;
    }

    void OnDestroy() {
        GameManager.instance.onSaveLoaded -= OnLoadedGame;
    }

    void OnLoadedGame() {
        if (!GameManager.instance.save.variables.HasVariable("initialStateChoice")) {
            Debug.Log("Escolha inicial de stats");
            MostrarEscolha(3).Then(() => {
                GameManager.instance.save.variables.SetVariable("initialStateChoice", true);
                GameManager.instance.save.Save();
            });
        }
    }

    public Promise MostrarEscolha(int pontos = 1) {
        GameManager.instance.Pausar();
        GameManager.instance.SetIntermediaryState(GameState.StatChoice);

        hadMouse = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pontosDisponiveisInicialmente = pontos;
        pontosRestantes = pontos;

        vida.SetPointsFixos(Player.instance.stats.vida);
        forca.SetPointsFixos(Player.instance.stats.forca);
        calor.SetPointsFixos(Player.instance.stats.calor);

        UpdateValues();

        holder.SetActive(true);

        openPromise = new Promise();
        return openPromise;
    }

    public void TentarConfirmarEscolha() {
        if (pontosRestantes == 0) {
            ConfirmarEscolha();
        }
    }

    public void ConfirmarEscolha() {
        GameManager.instance.RestoreIntermediaryState();

        if (!hadMouse) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Player.instance.stats.SetVida(vida.pontosFixos + vida.pontosExtras);
        Player.instance.stats.SetForca(forca.pontosFixos + forca.pontosExtras);
        Player.instance.stats.SetCalor(calor.pontosFixos + calor.pontosExtras);

        vida.SetPointsFixos(Player.instance.stats.vida);
        forca.SetPointsFixos(Player.instance.stats.forca);
        calor.SetPointsFixos(Player.instance.stats.calor);

        pontosDisponiveisInicialmente = 0;
        pontosRestantes = 0;
        UpdateValues();

        holder.SetActive(false);

        openPromise.Resolve();
        openPromise = null;
    }

    public void UpdateValues() {
        informaPontos.text = "Pontos restantes: " + pontosRestantes;
        confirmar.SetDisabled(pontosRestantes > 0);
        confirmar.GetComponent<Button>().interactable = pontosRestantes == 0;

        vida.Atualizar();
        forca.Atualizar();
        calor.Atualizar();
    }

    public void AdicionarPonto(StatChoicer stat) {
        if (pontosRestantes > 0) {
            pontosRestantes--;
            stat.Adicionar();
            UpdateValues();
        }
    }

    public void RemoverPonto(StatChoicer stat) {
        if (pontosRestantes < pontosDisponiveisInicialmente) {
            pontosRestantes++;
            stat.Remover();
            UpdateValues();
        }
    }
}
