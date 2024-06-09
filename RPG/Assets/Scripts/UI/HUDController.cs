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

    public void UpdateAtributos(int destreza, int forca, int vida, int calor) {
        atributosText.text = "Destreza: " + destreza + "\nForça: " + forca + "\nVida: " + vida + "\nCalor: " + calor;
    }

    public void UpdatePecas(int pecas) {
        pecasText.text = "" + pecas;
    }
}
