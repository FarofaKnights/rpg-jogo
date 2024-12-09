using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestFinalStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;

    bool finalizado = false;


    void Start() {
        OOutroFinal();
    }


    public void OOutroFinal() {
        if (finalizado) {
            return;
        }

        SceneManager.LoadScene(GameManager.instance.endSceneName);

        finalizado = true;
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        OOutroFinal();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Ativar final do jogo"; }
    #endif
}
