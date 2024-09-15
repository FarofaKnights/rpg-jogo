using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDeactivateStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string questObjectId;

    bool objectDeactivated = false;

    void Start() {
        DeactivateObject();
    }

    public void DeactivateObject() {
        if (objectDeactivated) {
            return;
        }

        QuestObject[] questObjects = FindObjectsOfType<QuestObject>(true);
        foreach (QuestObject questObject in questObjects) {
            if (questObject.questInfo == questInfo && questObject.objectId == questObjectId) {
                questObject.gameObject.SetActive(false);
            }
        }

        objectDeactivated = true;
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questObjectId = parameter;
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Desativar objeto"; }
    #endif
}
