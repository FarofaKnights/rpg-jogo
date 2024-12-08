using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayVideoStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string videoPath;

    bool set = false;


    void Start() {
        Set();
    }

    public void Set() {
        if (set) {
            return;
        }

        set = true;
        GameManager.instance.StartCoroutine(PlayAsync());
    }

    IEnumerator PlayAsync() {
        yield return UIController.video.PlayAsync(videoPath);
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;
        this.videoPath = parameter;

        Set();
    }

    public virtual bool IsEfeitoPersistente { get { return false; } }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Tocar Video"; }
    #endif
}
