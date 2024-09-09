using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState {
    WAITING_REQUIREMENTS,
    CAN_START,
    IN_PROGRESS,
    CAN_FINISH,
    FINISHED
}

public class Quest {
    public QuestInfo info;
    public QuestState state = QuestState.WAITING_REQUIREMENTS;
    int currentStep = 0;

    public Quest(QuestInfo info) {
        this.info = info;
    }

    public bool NextStep() {
        if (currentStep < info.steps.Length - 1) {
            currentStep++;
            return true;
        }

        return false;
    }

    public GameObject InstantiateStep(Transform parent) {
        if (currentStep >= info.steps.Length) {
            return null;
        }

        GameObjectParameter step = info.steps[currentStep];
        GameObject stepObject = GameObject.Instantiate(step.gameObject, parent);

        IQuestInformations questParameter = stepObject.GetComponent<IQuestInformations>();
        if (questParameter != null) {
            questParameter.HandleQuestInformations(info, step.parameter);
        }

        return stepObject;
    }
}
