using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour {
    public Image icon;
    public Image[] indicadores;
    public Color desativado, ativado;

    void Start() {
        icon.color = ativado;
        SetStat(1);
    }

    public void SetStat(int ativos) {
        for (int i = 0; i < indicadores.Length; i++) {
            if (i < ativos) {
                indicadores[i].color = ativado;
            } else {
                indicadores[i].color = desativado;
            }
        }
    }

    void OnValidate() {
        if (icon != null) icon.color = ativado;

        if (indicadores != null && indicadores.Length > 0) {
            SetStat(1);
        }
    }
}
