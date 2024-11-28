using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatChoicer : MonoBehaviour {
    public Image[] points;
    public Color corFixo, corAtivo, corDesativado;
    public GameObject adicionar, remover;

    public int pontosFixos = 0;
    public int pontosExtras = 0;

    public void SetPointsFixos(int pontos) {
        pontosFixos = pontos;
        pontosExtras = 0;
        Atualizar();
    }

    public void Atualizar() {
        for (int i = 0; i < points.Length; i++) {
            if (i < pontosFixos) {
                points[i].color = corFixo;
            } else if (i < pontosFixos + pontosExtras) {
                points[i].color = corAtivo;
            } else {
                points[i].color = corDesativado;
            }
        }

        bool cheio = pontosFixos + pontosExtras >= points.Length;

        adicionar.SetActive(!(cheio || UIController.statChoice.pontosRestantes == 0));
        remover.SetActive(pontosExtras > 0);
    }

    public void TentarAdicionar() {
        UIController.statChoice.AdicionarPonto(this);
    }

    public void Adicionar() {
        pontosExtras++;
        Atualizar();
    }

    public void TentarRemover() {
        UIController.statChoice.RemoverPonto(this);
    }

    public void Remover() {
        pontosExtras--;
        Atualizar();
    }
}
