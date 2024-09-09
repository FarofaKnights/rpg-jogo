using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uma classe que representa um passo de uma missão
public abstract class QuestStep : MonoBehaviour {
    bool isFinished = false;
    string questId;

    public void Initialize(string questId) {
        this.questId = questId;
    }

    protected void FinishStep() {
        if (!isFinished) {
            isFinished = true;
            QuestManager.instance.AdvanceQuest(questId);
            Destroy(gameObject);
        }
    }
}


