using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestTargetPlayerStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string groupID;
    bool alreadySet = false;
    EnemyGroup currentEnemyGroup;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;
        bool found = false;

        if (groupID != "") {
            EnemyGroup[] enemyGroups = FindObjectsOfType<EnemyGroup>(true);
            foreach (EnemyGroup enemyGroup in enemyGroups) {
                if (enemyGroup.groupID == groupID) {
                    found = true;
                    currentEnemyGroup = enemyGroup;
                    break;
                }
            }
        }

        if (!found) {
            Debug.LogError("Grupo de inimigos não encontrado: " + groupID);
            FinishStep();
            return;
        }


        foreach (Inimigo inimigo in currentEnemyGroup.inimigos) {
            inimigo.SetTarget(Player.instance.gameObject);
        }

        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        this.groupID = parameter;

        Set();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "Ação: Targetar o Player"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string groupID = stepInfo.step.parameter ?? "";

        groupID = EditorGUILayout.TextField("EnemyGroup GroupID", groupID);

        return JoinParameters(new string[] { groupID });
    }
    #endif
}
