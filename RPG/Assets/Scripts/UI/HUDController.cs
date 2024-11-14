using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Color calorPositivoColor, calorNegativoColor;
    public Image calorSliderFill;

    public Text pecasText;
    public Image imagemArma, imagemSave, playerAim;
    public Color normalAimColor, targetAimColor;

    [Header("Boss")]
    public GameObject objetivoPrefab;
    public GameObject objetivoHolder;

    [Header("Outros")]
    public Slider bossVidaSlider;
    public GameObject fpsGO;

    class Missao_Texto {
        public string texto;
        public QuestInfo questInfo;

        public GameObject instancia;
        public Text tituloText, descricaoText;

        public Missao_Texto(QuestInfo questInfo, string texto) {
            this.questInfo = questInfo;
            this.texto = texto;
            this.instancia = GameObject.Instantiate(UIController.HUD.objetivoPrefab, UIController.HUD.objetivoHolder.transform);

            Text[] textos = instancia.GetComponentsInChildren<Text>();

            foreach (Text textoEl in textos) {
                if (textoEl.name == "Titulo") {
                    this.tituloText = textoEl;
                } else if (textoEl.name == "Descricao") {
                    this.descricaoText = textoEl;
                }
            }

            this.tituloText.text = questInfo.titulo;
            this.descricaoText.text = texto;
        }

        public void Update(string texto) {
            this.texto = texto;
            this.descricaoText.text = texto;
        }

        public void Destroy() {
            GameObject.Destroy(instancia);
        }
    }

    List<Missao_Texto> missoesTextos = new List<Missao_Texto>();

    void Awake() {
        foreach (Transform filho in objetivoHolder.transform) {
            Destroy(filho.gameObject);
        }
    }

    public void UpdateVida(float vida, float vidaMax) {
        vidaSlider.maxValue = vidaMax;
        vidaSlider.value = vida;
    }

    public void UpdateCalor(float calor, float calorMax) {
        calorSlider.maxValue = calorMax;
        calorSlider.value = Mathf.Abs(calor);

        if (calor > 0) {
            calorSliderFill.color = calorPositivoColor;
        } else {
            calorSliderFill.color = calorNegativoColor;
        }
    }

    public void UpdatePecas(int pecas) {
        pecasText.text = "" + pecas;
    }

    
    public void UpdateMissaoText(QuestInfo quest, string texto) {
        bool achou = false;
        foreach (Missao_Texto mt in missoesTextos) {
            if (mt.questInfo == quest) {
                mt.Update(texto);
                achou = true;
                break;
            }
        }

        if (!achou) {
            missoesTextos.Add(new Missao_Texto(quest, texto));
        }

        for (int i = missoesTextos.Count - 1; i >= 0; i--) {
            if (missoesTextos[i].texto == "" || QuestManager.instance.IsQuestFinished(missoesTextos[i].questInfo.questId)) {
                missoesTextos[i].Destroy();
                missoesTextos.RemoveAt(i);
            }
        }
    }

    public void UpdateMissaoText() {
        for (int i = missoesTextos.Count - 1; i >= 0; i--) {
            Missao_Texto mt = missoesTextos[i];
            string texto = mt.texto;
            mt.Update(texto);

            if (texto == "" || QuestManager.instance.IsQuestFinished(mt.questInfo.questId)) {
                mt.Destroy();
                missoesTextos.RemoveAt(i);
            }
        }
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
