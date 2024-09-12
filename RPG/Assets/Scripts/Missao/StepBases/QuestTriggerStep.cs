using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTriggerStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string triggerName;
    bool alreadySet = false;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;
        QuestManager.instance.RegisterQuestTrigger(questId, triggerName, OnTrigger);
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;
        this.triggerName = parameter;

        Set();
    }

    public void OnTrigger() {
        QuestManager.instance.ClearQuestTriggers(questId, triggerName);
        FinishStep();
    }
}
