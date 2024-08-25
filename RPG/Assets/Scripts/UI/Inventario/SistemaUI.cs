using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaUI : MonoBehaviour, UITab {
    public void Continuar() {
        UIController.instance.HideMenu();
    }

    public void Sair() {
        GameManager.instance.GameStart();
    }

    public void SairJogo() {
        Application.Quit();
    }

    public void VoltarSave() {
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
