using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Uma classe que representa um passo de uma missão
public abstract class QuestStep : MonoBehaviour {
    bool isFinished = false;
    protected string questId;

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

    public string[] SeparateParameters(string parameters) {
        return parameters.Split(';');
    }

    public string JoinParameters(string[] parameters) {
        return string.Join(";", parameters);
    }

    #if UNITY_EDITOR

    public abstract string GetEditorName();

    public virtual string GetEditorParameters(CurrentStepAcaoInfo info) {
        return EditorGUILayout.TextField("Parâmetro", info.step.parameter);
    }

    public virtual void SetEditorParameters(CurrentStepAcaoInfo info, string parameters) {
        info.step.parameter = parameters;
    }

    public virtual void OnStepDelete(CurrentStepAcaoInfo info) { }

    #endif
}


