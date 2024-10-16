using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActivateStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string questObjectId;

    bool objectActivated = false;


    void Start() {
        ActivateObject();
    }

    public override bool IsEfeitoPersistente { get { return true; } }

    public void ActivateObject() {
        if (objectActivated) {
            return;
        }

        QuestObject[] questObjects = FindObjectsOfType<QuestObject>(true);
        foreach (QuestObject questObject in questObjects) {
            if (questObject.questInfo == questInfo && questObject.objectId == questObjectId) {
                questObject.gameObject.SetActive(true);
            }
        }

        objectActivated = true;
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;
        this.questObjectId = parameter;

        ActivateObject();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Ativar objeto"; }
    #endif
}
