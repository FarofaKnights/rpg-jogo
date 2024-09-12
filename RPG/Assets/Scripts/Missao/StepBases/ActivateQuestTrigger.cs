using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Objeto que são ativados/desativados de acordo com o estado de uma quest
public class ActivateQuestTrigger : MonoBehaviour {
    [Header("Activate Quest Trigger Info")]
    public QuestInfo questInfo;
    public string triggerName;
    public bool activateOnTriggerEnter = false;

    public void Activate() {
        QuestManager.instance.TriggerQuest(questInfo.questId, triggerName);
    }

    void OnTriggerEnter(Collider other) {
        if (activateOnTriggerEnter && other.CompareTag("Player")) {
            Activate();
        }
    }
}
