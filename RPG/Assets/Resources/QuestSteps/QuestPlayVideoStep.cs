using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestPlayVideoStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string videoPath;
    public bool fadeOut = true;

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
        yield return UIController.video.PlayAsync(videoPath, fadeOut);
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        this.videoPath = parameters[0];
        this.fadeOut = parameters.Length > 1 ? bool.Parse(parameters[1]) : true;

        Set();
    }

    public override bool IsEfeitoPersistente { get { return false; } }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Tocar Video"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter);

        string videoPath = parameters.Length > 0 ? parameters[0] : "";
        bool fadeOut = parameters.Length > 1 ? bool.Parse(parameters[1]) : true;

        videoPath = EditorGUILayout.TextField("Video", videoPath);
        fadeOut = EditorGUILayout.Toggle("Fade Out", fadeOut);

        return JoinParameters(new string[] { videoPath, fadeOut.ToString() });
    }
    #endif
}
