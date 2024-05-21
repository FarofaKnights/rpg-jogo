using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfiguracoesUI : MonoBehaviour, UITab {
    public void Continuar() {
        UIController.instance.HideMenu();
    }

    public void Sair() {
        GameManager.instance.GameStart();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
