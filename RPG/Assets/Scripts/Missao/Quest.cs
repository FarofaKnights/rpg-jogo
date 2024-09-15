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
    int internalStep = 0; // Para steps que são pais

    public Quest(QuestInfo info) {
        this.info = info;
    }

    public bool NextStep() {
        if (currentStep >= info.steps.Length) {
            return false;
        }

        GameObjectParameter step = info.steps[currentStep];
        if (step.type == QuestStepType.PAI) {
            GameObjectParameterParent parent = (GameObjectParameterParent)step;

            if (internalStep < parent.children.Length - 1) {
                internalStep++;
                return true;
            }
            internalStep = 0;
        }

        if (currentStep < info.steps.Length - 1) {
            currentStep++;
            return true;
        }

        return false;
    }

    public GameObject InstantiateStep(Transform parent) {
        if (currentStep >= info.steps.Length) {
            UIController.HUD.UpdateMissaoText("");
            return null;
        }

        GameObjectParameter step = info.steps[currentStep];

        if (step.type == QuestStepType.PAI) {
            return HandlePai((GameObjectParameterParent)step, parent);
        } else if (step.type == QuestStepType.ACAO) {
            return HandleAcao(step, parent);
        }

        return null;
    }

    GameObject HandleAcao(GameObjectParameter step, Transform parent) {
        GameObject stepObject = GameObject.Instantiate(step.gameObject, parent);

        IQuestInformations questParameter = stepObject.GetComponent<IQuestInformations>();
        if (questParameter != null) {
            questParameter.HandleQuestInformations(info, step.parameter);
            UIController.HUD.UpdateMissaoText(CurrentMessage());
        }

        return stepObject;
    }

    GameObject HandlePai(GameObjectParameterParent parentStep, Transform parent) {
        GameObjectParameter step = parentStep.children[internalStep];
        return HandleAcao(step, parent);
    }

    public string CurrentMessage() {
        if (state == QuestState.WAITING_REQUIREMENTS || state == QuestState.CAN_START) {
            return "";
        }

        if (currentStep < info.steps.Length) {
            if (isCurrentStepParent()) {
                return info.steps[currentStep].children[internalStep].informativo;
            }
            return info.steps[currentStep].informativo;
        }

        return "";
    }

    public string[] AllMessagesUntillNow() {
        bool acabou = false;
        if (state == QuestState.WAITING_REQUIREMENTS || state == QuestState.CAN_START) {
            return new string[0];
        } else if (state == QuestState.CAN_FINISH || state == QuestState.FINISHED) {
            acabou = true;
        }

        List<string> messages = new List<string>();

        for (int i = 0; i <= currentStep; i++) {
            GameObjectParameter step = info.steps[i];
            string inf = "";

            if (step.type == QuestStepType.PAI) {
                GameObjectParameterParent parent = (GameObjectParameterParent)step;

                int size = i == currentStep && !acabou ? internalStep : parent.children.Length - 1;
                for (int j = 0; j <= size; j++) {                 
                    inf = parent.children[j].informativo;
                    if (inf != null && inf != "") messages.Add(inf);
                }
            } else {
                inf = step.informativo;
                if (inf != "")  messages.Add(inf);
            } 
        }

        return messages.ToArray();
    }

    bool isCurrentStepParent() {
        return info.steps[currentStep].type == QuestStepType.PAI;
    }
}
