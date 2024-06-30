using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Text atributosText;
    public Text pecasText;
    public Image imagemArma, imagemSave;

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

    public void SetArmaEquipada(Arma arma) {
        if (arma == null) {
            imagemArma.sprite = null;
            imagemArma.color = new Color(0, 0, 0, 0);
            return;
        }

        Item item = arma.GetComponent<Item>();
        if (item == null || item.data == null) {
            Debug.LogError("Arma não tem componente Item ou Item não tem data!");
            imagemArma.sprite = null;
            imagemArma.color = new Color(0, 0, 0, 0);
            return;
        }

        ItemData data = item.data;
        imagemArma.sprite = data.sprite;
        imagemArma.color = Color.white;
    }

    public void PopUpSaveInfo() {
        StartCoroutine(PopUpSaveInfoCoroutine());
    }

    public IEnumerator PopUpSaveInfoCoroutine() {
        imagemSave.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        imagemSave.gameObject.SetActive(false);
    }
}
