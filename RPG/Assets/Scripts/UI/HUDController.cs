using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Text pecasText, missaoText;
    public Image imagemArma, imagemSave, playerAim;
    public Color normalAimColor, targetAimColor;

    void Start() {
        UpdateMissaoText("");
    }

    public void UpdateVida(float vida, float vidaMax) {
        vidaSlider.maxValue = vidaMax;
        vidaSlider.value = vida;
    }

    public void UpdateCalor(float calor, float calorMax) {
        calorSlider.maxValue = calorMax;
        calorSlider.value = calor;
    }

    public void UpdatePecas(int pecas) {
        pecasText.text = "" + pecas;
    }

    public void UpdateMissaoText(string texto) {
        missaoText.text = texto;
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

    public void ShowAim(bool show) {
        playerAim.gameObject.SetActive(show);
    }

    public void AimHasTarget(bool hasTarget) {
        playerAim.color = hasTarget ? targetAimColor : normalAimColor;
    }

    void OnValidate() {
        if (playerAim != null) {
            playerAim.color = normalAimColor;
        }
    }
}
