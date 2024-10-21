using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Text pecasText, missaoText;
    public Image imagemArma, imagemSave, playerAim;
    public Color normalAimColor, targetAimColor;

    public Slider bossVidaSlider;
    public GameObject fpsGO;

    class Missao_Texto {
        public string texto;
        public QuestInfo questInfo;

        public Missao_Texto(QuestInfo questInfo, string texto) {
            this.questInfo = questInfo;
            this.texto = texto;
        }
    }

    List<Missao_Texto> missoesTextos = new List<Missao_Texto>();

    void Start() {
        missaoText.text = "";
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

    

    public void UpdateMissaoText(QuestInfo quest, string texto) {
        bool achou = false;
        foreach (Missao_Texto mt in missoesTextos) {
            if (mt.questInfo == quest) {
                mt.texto = texto;
                achou = true;
                break;
            }
        }

        if (!achou) {
            missoesTextos.Add(new Missao_Texto(quest, texto));
        }

        UpdateMissaoText();
    }

    public void UpdateMissaoText() {
        for (int i = missoesTextos.Count - 1; i >= 0; i--) {
            if (missoesTextos[i].texto == "" || QuestManager.instance.IsQuestFinished(missoesTextos[i].questInfo.questId)) {
                missoesTextos.RemoveAt(i);
            }
        }

        string missao = "";
        foreach (Missao_Texto mt in missoesTextos) {
            missao += "- " + mt.texto + "\n";
        }

        missaoText.text = missao;
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

    public void ShowBossVida(bool show) {
        bossVidaSlider.gameObject.SetActive(show);
    }

    public void UpdateBossVida(float vida, float vidaMax) {
        bossVidaSlider.maxValue = vidaMax;
        bossVidaSlider.value = vida;
    }
}
