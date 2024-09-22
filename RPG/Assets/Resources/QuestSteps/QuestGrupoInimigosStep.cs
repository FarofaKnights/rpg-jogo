using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestGrupoInimigosStep : QuestStep, IQuestInformations, IInformativoAtualizavel {
    public QuestInfo questInfo;
    public string groupID;
    public bool showQuantity = true;
    bool alreadySet = false;
    EnemyGroup currentEnemyGroup;
    System.Action informativoUpdate;

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
                    
                    currentEnemyGroup.onQuantityChange += OnDeadEnemyUpdate;
                    currentEnemyGroup.onGroupDeath += FinishStep;
                    if (currentEnemyGroup.IsDead) FinishStep();
                    break;
                }
            }
        }

        if (!found) {
            Debug.LogError("Grupo de inimigos não encontrado: " + groupID);
            FinishStep();
        }
    }

    void OnDeadEnemyUpdate(int aliveEnemies) {
        informativoUpdate?.Invoke();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        this.groupID = parameters[0];
        this.showQuantity = parameters.Length > 1 ? bool.Parse(parameters[1]) : showQuantity;

        Set();
    }

    public override void OnEnd() {
        if (currentEnemyGroup != null && currentEnemyGroup.gameObject != null) {
            currentEnemyGroup.onGroupDeath -= FinishStep;
            currentEnemyGroup.onQuantityChange -= OnDeadEnemyUpdate;
        }
    }

    public override Dictionary<string, string> GetDynamicInfo() {
        if (currentEnemyGroup == null) return new Dictionary<string, string> { };
        return new Dictionary<string, string> {
            { "vivos", currentEnemyGroup.AliveEnemies.ToString() },
            { "mortos", (currentEnemyGroup.TotalEnemies - currentEnemyGroup.AliveEnemies).ToString() },
            { "total", currentEnemyGroup.TotalEnemies.ToString() }
        };
    }

    public override string AddToInformativo(string informativo) {
        if (currentEnemyGroup == null || !showQuantity) return informativo;
        return informativo += " ({mortos}/{total})";
    }

    public void UpdateInformativo(System.Action informativoUpdate) {
        this.informativoUpdate = informativoUpdate;
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "CONDIÇÃO: Grupo de Inimigos"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter);
        string groupID = parameters.Length > 0 ? parameters[0] : "";
        bool showQuantity = parameters.Length > 1 ? bool.Parse(parameters[1]) : true;

        groupID = EditorGUILayout.TextField("EnemyGroup GroupID", groupID);
        showQuantity = EditorGUILayout.Toggle("Show Quantity", showQuantity);




        return JoinParameters(new string[] { groupID, showQuantity.ToString() });
    }
    #endif
}
