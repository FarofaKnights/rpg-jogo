using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissaoUI : MonoBehaviour, UITab {
    [Header("Static Components")]
    public Text titulo;
    public Text descricao, statusMissaoAtual;
    public Transform missaoConteudoHolder, missaoBtnHolder;
    public Color missaoTextoConcluido;

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
        statusMissaoAtual.text = "";
        
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

        int i = 0;
        bool encerrada = false;

        switch(quest.state) {
            case QuestState.WAITING_REQUIREMENTS:
                statusMissaoAtual.text = "Aguardando requisitos";
                break;
            case QuestState.CAN_START:
                statusMissaoAtual.text = "Pode começar";
                break;
            case QuestState.IN_PROGRESS:
                statusMissaoAtual.text = "Missão em progresso";
                break;
            case QuestState.CAN_FINISH:
                statusMissaoAtual.text = "Pode finalizar";
                encerrada = true;
                break;
            case QuestState.FINISHED:
                statusMissaoAtual.text = "Missão concluída";
                encerrada = true;
                break;
        }

        string[] messages = quest.AllMessagesUntillNow();
        foreach (string message in messages) {
            GameObject text = Instantiate(missaoTextPrefab, missaoConteudoHolder);
            GameObject divisoria = Instantiate(divisoriaPrefab, missaoConteudoHolder);
            text.GetComponent<Text>().text = "- " + message;
            if (i < messages.Length - 1 || encerrada) {
                text.GetComponent<Text>().color = missaoTextoConcluido;
            }
            i++;
        }
        
    }
}
