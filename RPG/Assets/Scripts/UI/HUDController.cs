using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Slider vidaSlider, calorSlider;
    public Color calorPositivoColor, calorNegativoColor;
    public Image calorSliderFill;

    public Text pecasText;
    public Image imagemSave, playerAim;
    public Color normalAimColor, targetAimColor;
    public IndicadorMiraUI[] indicadoresMira;

    [Header("Indicadores de equipamento")]
    public Image imagemArma;
    public Image imagemBraco;
    public GameObject principalHolder, secundariaHolder;

    [Header("Missão")]
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

        ShowAim(false);
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

    void SetEquipamento(Image imagem, Item item) {
        if (item == null || item.data == null) {
            imagem.sprite = null;
            imagem.color = new Color(0, 0, 0, 0);
            return;
        }

        ItemData data = item.data;
        imagem.sprite = data.sprite;
        imagem.color = Color.white;
    }

    public void SetArmaEquipada(Arma arma) {
        if (arma == null) {
            SetEquipamento(imagemArma, null);
            return;
        }

        Item item = arma.GetComponent<Item>();
        SetEquipamento(imagemArma, item);
    }

    public void SetBracoEquipado(Braco braco) {
        if (braco == null) {
            SetEquipamento(imagemBraco, null);
            return;
        }

        Item item = braco.GetComponent<Item>();
        SetEquipamento(imagemBraco, item);
    }

    public void PopUpSaveInfo() {
        StartCoroutine(PopUpSaveInfoCoroutine());
    }

    public IEnumerator PopUpSaveInfoCoroutine() {
        imagemSave.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        imagemSave.gameObject.SetActive(false);
    }

    public void SetBracoMode(bool isBracoMode) {
        if (isBracoMode) {
            imagemBraco.transform.SetParent(principalHolder.transform);
            imagemArma.transform.SetParent(secundariaHolder.transform);
        } else {
            imagemArma.transform.SetParent(principalHolder.transform);
            imagemBraco.transform.SetParent(secundariaHolder.transform);
        }

        imagemArma.rectTransform.anchoredPosition = Vector2.zero;
        imagemArma.rectTransform.offsetMin = Vector2.zero;
        imagemArma.rectTransform.offsetMax = Vector2.zero;
        imagemBraco.rectTransform.anchoredPosition = Vector2.zero;
        imagemBraco.rectTransform.offsetMin = Vector2.zero;
        imagemBraco.rectTransform.offsetMax = Vector2.zero;

        ShowAim(isBracoMode);
    }

    public void ShowAim(bool show) {
        // playerAim.gameObject.SetActive(show);

        foreach (IndicadorMiraUI indicador in indicadoresMira) {
            indicador.gameObject.SetActive(show);
            indicador.enabled = show;
        }
    }

    public void AimHasTarget(bool hasTarget) {
        // playerAim.color = hasTarget ? targetAimColor : normalAimColor;

        foreach (IndicadorMiraUI indicador in indicadoresMira) {
            indicador.SetAim(hasTarget);
            indicador.color = hasTarget ? targetAimColor : normalAimColor;
        }
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
