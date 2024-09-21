using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoint : MonoBehaviour {    public QuestInfo questInfo;

    QuestState questState;

    void Start() {
        QuestManager.instance.OnQuestStateChanged += OnQuestStateChanged;

        questState = QuestManager.instance.GetQuestState(questInfo.questId);
        gameObject.SetActive(questState == QuestState.CAN_START);
    }

    // Mantem questState sempre atualizado com o estado da quest do ponto
    void OnQuestStateChanged(Quest quest) {
        if (quest.info.questId == questInfo.questId) {
            questState = quest.state;

            if (questState == QuestState.CAN_START) gameObject.SetActive(true);
            else gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        if (questState == QuestState.CAN_START) {
            QuestManager.instance.StartQuest(questInfo.questId);
            gameObject.SetActive(false);
        }
    }
}
