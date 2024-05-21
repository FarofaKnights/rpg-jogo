using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Text atributosText;
    public Text pecasText;

    public void UpdateVida(float vida, float vidaMax) {
        vidaSlider.maxValue = vidaMax;
        vidaSlider.value = vida;
    }

    public void UpdateCalor(float calor, float calorMax) {
        calorSlider.maxValue = calorMax;
        calorSlider.value = calor;
    }

    public void UpdateAtributos(int dano, int defesa, int velocidade) {
        atributosText.text = "Dano: " + dano + "\nDefesa: " + defesa + "\nVelocidade: " + velocidade;
    }

    public void UpdatePecas(int pecas) {
        pecasText.text = "" + pecas;
    }
}
