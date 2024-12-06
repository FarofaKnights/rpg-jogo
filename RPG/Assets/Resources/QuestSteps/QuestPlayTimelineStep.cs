using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayTimelineStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string timelineId;

    bool set = false;


    void Start() {
        Set();
    }

    public void Set() {
        if (set) {
            return;
        }

        set = true;

        TimelineObject targetRef = null;

        foreach (TimelineObject cutsceneRef in FindObjectsOfType<TimelineObject>()) {
            if (cutsceneRef.timelineId == timelineId) {
                targetRef = cutsceneRef;
                break;
            }
        }

        Cutscene target = targetRef?.cutscene;

        if (target == null) {
            Debug.LogError("Cutscene não encontrada: " + timelineId);
            FinishStep();
            return;
        }

        target.onEnd += FinishStep;
        target.Comecar();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;
        this.timelineId = parameter;

        Set();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Tocar Timeline"; }
    #endif
}
