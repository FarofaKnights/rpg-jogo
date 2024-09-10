using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissaoUI : MonoBehaviour, UITab {
    [Header("Static Components")]
    public Text titulo;
    public Text descricao;
    public Transform missaoConteudoHolder, missaoBtnHolder;

    [Header("Prefabs")]
    public GameObject missaoBtnPrefab;
    public GameObject missaoTextPrefab, divisoriaPrefab;

    public void Show() {
        CarregarBotoes();
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void CarregarBotoes() {
        foreach (Transform child in missaoBtnHolder) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in missaoConteudoHolder) {
            Destroy(child.gameObject);
        }

        foreach (Quest quest in QuestManager.instance.quests.Values) {
            GameObject btn = Instantiate(missaoBtnPrefab, missaoBtnHolder);
            btn.GetComponentInChildren<Text>().text = quest.info.titulo;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                CarregarMissao(quest);
            });
        }

        titulo.text = "";
        descricao.text = "";
        
        foreach (Transform child in missaoConteudoHolder) {
            Destroy(child.gameObject);
        }
    }

    public void CarregarMissao(Quest quest) {
        titulo.text = quest.info.titulo;
        descricao.text = quest.info.descricao;

        foreach (Transform child in missaoConteudoHolder) {
            Destroy(child.gameObject);
        }
    }
}
