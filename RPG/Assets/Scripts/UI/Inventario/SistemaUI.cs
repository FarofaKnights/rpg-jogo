using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaUI : MonoBehaviour, UITab {
    public void Continuar() {
        UIController.instance.HideMenu();
    }

    public void Sair() {
        UIController.modal.OpenModal("Você tem certeza que deseja voltar para o menu?");
        UIController.modal.OnYes(SairConfirmed, false);
    }

    void SairConfirmed() {
        GameManager.instance.GameStart();
    }

    public void SairJogo() {
        UIController.modal.OpenModal("Você tem certeza que deseja sair do jogo?");
        UIController.modal.OnYes(SairJogoConfirmed, false);
    }

    void SairJogoConfirmed() {
        Application.Quit();
    }

    public void VoltarSave() {
        UIController.modal.OpenModal("Você tem certeza que deseja resetar o save?");
        UIController.modal.OnYes(VoltarSaveConfirmed, false);
    }

    void VoltarSaveConfirmed() {
        UIController.instance.CarregarJogo();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void ChangeYInvert()
    {
        SettingsManager.instance.ChangeInvertY();
    }

    public void ChangeXInvert()
    {
        SettingsManager.instance.ChangeInvertX();
    }
}
