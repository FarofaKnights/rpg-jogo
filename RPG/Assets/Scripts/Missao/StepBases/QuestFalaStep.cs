using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFalaStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string falaName;
    bool alreadySet = false;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;
        Fala[] falas = questInfo.GetFalas(falaName);

        if (falas != null && falas.Length > 0)
            UIController.dialogo.StartDialogo(falas, OnTrigger);
        else
            FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.falaName = parameter;

        Set();
    }

    public void OnTrigger() {
        FinishStep();
    }
}
