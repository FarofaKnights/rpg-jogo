using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestWaitSecondsStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public float seconds;

    bool set = false;


    void Start() {
        Set();
    }

    public void Set() {
        if (set) {
            return;
        }

        set = true;
        StartCoroutine(WaitSeconds());
    }

    IEnumerator WaitSeconds() {
        yield return new WaitForSeconds(seconds);
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;
        this.seconds = float.Parse(parameter);

        Set();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "CONDIÇÃO: Espera segundos"; }
    #endif
}
