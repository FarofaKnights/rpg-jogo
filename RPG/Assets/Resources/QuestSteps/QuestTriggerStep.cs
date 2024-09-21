using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestTriggerStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string triggerName;
    public string triggerHolder;
    bool alreadySet = false;

    public override bool IsEfeitoPersistente { get { return true; } }

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;

        if (triggerHolder != "") {
             QuestObject[] questObjects = FindObjectsOfType<QuestObject>(true);
            foreach (QuestObject questObject in questObjects) {
                if (questObject.questInfo == questInfo && questObject.objectId == triggerHolder) {
                    questObject.gameObject.SetActive(true);
                }
            }
        }
        QuestManager.instance.RegisterQuestTrigger(questId, triggerName, FinishStep);
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        this.triggerName = parameters[0];
        if (parameters.Length > 1) {
            this.triggerHolder = parameters[1];
        }

        Set();
    }

    public override void OnEnd() {
        QuestManager.instance.ClearQuestTriggers(questId, triggerName);

        if (triggerHolder != "") {
             QuestObject[] questObjects = FindObjectsOfType<QuestObject>(true);
            foreach (QuestObject questObject in questObjects) {
                if (questObject.questInfo == questInfo && questObject.objectId == triggerHolder) {
                    questObject.gameObject.SetActive(false);
                }
            }
        }
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "CONDIÇÃO: Trigger"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter);
        string nomeDoTrigger = parameters.Length > 0 ? parameters[0] : "";
        nomeDoTrigger = EditorGUILayout.TextField("Nome do trigger", nomeDoTrigger);

        string holder = parameters.Length > 1 ? parameters[1] : "";
        holder = EditorGUILayout.TextField("Objeto do trigger (opcional)", holder);

        return JoinParameters(new string[] { nomeDoTrigger, holder });
    }
    #endif
}
