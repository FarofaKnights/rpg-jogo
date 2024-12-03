using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestChangeScene : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string sceneName;
    public string point;

    bool sceneChanged = false;

    void Start() {
        ChangeScene();
    }

    public void ChangeScene() {
        if (sceneChanged) return;
        sceneChanged = true;

        LevelInfo level = GameManager.instance.loading.GetLevelInfo(sceneName);
        GameManager.instance.GoToScene(level);

        // O mudar de cena é uma corrotina, portanto vai terminar o passo antes de mudar de cena
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        this.sceneName = parameters[0];
        this.point = parameters.Length > 1 ? parameters[1] : "";

        ChangeScene();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: NÃO USAR AINDA Mudar cena"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter);

        string cena = parameters.Length > 0 ? parameters[0] : "";
        string point = parameters.Length > 1 ? parameters[1] : "";

        cena = EditorGUILayout.TextField("Cena", cena);
        point = EditorGUILayout.TextField("Spawnpoint", point);

        return JoinParameters(new string[] { cena, point });
    }
    #endif
}
