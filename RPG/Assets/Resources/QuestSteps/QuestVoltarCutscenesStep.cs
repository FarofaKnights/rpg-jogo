using UnityEngine;

public class QuestVoltarCutscenesStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;

    bool mudou = false;


    void Start() {
        Retorna();
    }


    public void Retorna() {
        if (mudou) {
            return;
        }

        GameManager.instance.ReturnToCutscenesScreen();

        mudou = true;
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        Retorna();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Retorna p tela cutscenes"; }
    #endif
}
