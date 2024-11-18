using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public enum QuestState {
    WAITING_REQUIREMENTS,
    CAN_START,
    IN_PROGRESS,
    CAN_FINISH,
    FINISHED
}

public class Quest : Saveable {
    public QuestInfo info;
    public QuestState state = QuestState.WAITING_REQUIREMENTS;
    int currentStep = 0;
    int internalStep = 0; // Para steps que são pais
    QuestStep currentStepObject;

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
            UIController.HUD.UpdateMissaoText(info, "");
            return null;
        }

        GameObjectParameter step = info.steps[currentStep];

        if (step.type == QuestStepType.PAI) {
            return HandlePai((GameObjectParameterParent)step, parent);
        } else if (step.type == QuestStepType.PADRAO) {
            return HandleAcao(step, parent);
        }

        return null;
    }

    GameObject HandleAcao(GameObjectParameter step, Transform parent) {
        GameObject stepObject = GameObject.Instantiate(step.gameObject, parent);
        currentStepObject = stepObject.GetComponent<QuestStep>();

        IQuestInformations questParameter = stepObject.GetComponent<IQuestInformations>();
        if (questParameter != null) {
            questParameter.HandleQuestInformations(info, step.parameter);
        }

        IInformativoAtualizavel informativo = stepObject.GetComponent<IInformativoAtualizavel>();
        if (informativo != null) {
            informativo.UpdateInformativo(() => {
                UIController.HUD.UpdateMissaoText(info, CurrentMessage());
            });
        }

        UIController.HUD.UpdateMissaoText(info, CurrentMessage());

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
            string message = info.steps[currentStep].informativo;
            if (isCurrentStepParent()) {
                message = info.steps[currentStep].children[internalStep].informativo;
            }

            message = currentStepObject.AddToInformativo(message);

            Dictionary<string, string> dynamicInfo = currentStepObject.GetDynamicInfo();
            foreach (KeyValuePair<string, string> entry in dynamicInfo) {
                message = message.Replace("{" + entry.Key + "}", entry.Value);
            }

            return message;
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

    public JSONObject Save() {
        JSONObject obj = new JSONObject();
        obj.AddField("state", (int)state);
        if (state == QuestState.IN_PROGRESS) {
            obj.AddField("currentStep", currentStep);
            obj.AddField("internalStep", internalStep);
        }
        return obj;
    }

    public void Load(JSONObject obj) {
        QuestState oldState = state;
        QuestState atEndState = (QuestState)obj.GetField("state").intValue;

        if (oldState == QuestState.IN_PROGRESS && (atEndState == QuestState.CAN_START || atEndState == QuestState.WAITING_REQUIREMENTS)) {
            return;
        }
        
        if (oldState == QuestState.WAITING_REQUIREMENTS && obj.GetField("state").intValue > 0) {
            Condicao condicao = info.requirementsInfo.GetCondicao();
            condicao.Clear();
        }

        if (obj.GetField("state").intValue < 2) {
            QuestManager.instance.ChangeQuestState(info.questId, atEndState);
            return;
        }

        int target_currentStep = obj.GetField("currentStep") != null ? (int)obj.GetField("currentStep").intValue : 0;
        int target_internalStep = obj.GetField("internalStep") != null ? (int)obj.GetField("internalStep").intValue : 0;

        if (atEndState == QuestState.CAN_FINISH || atEndState == QuestState.FINISHED) {
            target_currentStep = info.steps.Length - 1;
            if (info.steps[target_currentStep].type == QuestStepType.PAI) {
                target_internalStep = ((GameObjectParameterParent)info.steps[target_currentStep]).children.Length - 1;
            } else {
                target_internalStep = 0;
            }
        }

        currentStep = 0;
        internalStep = 0;
        QuestManager.instance.ChangeQuestState(info.questId, QuestState.IN_PROGRESS);

        // Percorre pelos passos, executando passos persistentes
        while(!(currentStep == target_currentStep && internalStep == target_internalStep)) {
            QuestStep questStep = null;
            if (info.steps[currentStep].type == QuestStepType.PAI) {
                GameObjectParameterParent parent = (GameObjectParameterParent)info.steps[currentStep];
                questStep = parent.children[internalStep].gameObject.GetComponent<QuestStep>();
            } else {
                questStep = info.steps[currentStep].gameObject.GetComponent<QuestStep>();
            }

            if (questStep.IsEfeitoPersistente) {
                QuestManager.instance.RunQuestStepOnLoad(info.questId);
                currentStepObject.DummyFinishStep();
            }

            NextStep();
        }

        // Carrega o ultimo passo
        QuestStep ultimoStep = null;
        if (info.steps[currentStep].type == QuestStepType.PAI) {
            GameObjectParameterParent parent = (GameObjectParameterParent) info.steps[currentStep];
            ultimoStep = parent.children[internalStep].gameObject.GetComponent<QuestStep>();
        } else info.steps[currentStep].gameObject.GetComponent<QuestStep>();

        
        if (atEndState != QuestState.CAN_START && (atEndState == QuestState.IN_PROGRESS || (ultimoStep != null && ultimoStep.IsEfeitoPersistente)))
            QuestManager.instance.RunQuestStepOnLoad(info.questId);
        QuestManager.instance.ChangeQuestState(info.questId, atEndState);
        
        UIController.HUD.UpdateMissaoText(info, CurrentMessage());
    }

    QuestStep CurrentStepObject() {
        if (currentStep >= info.steps.Length) {
            return null;
        }

        GameObjectParameter step = info.steps[currentStep];
        if (step.type == QuestStepType.PAI) {
            return ((GameObjectParameterParent)step).children[internalStep].gameObject.GetComponent<QuestStep>();
        }

        return step.gameObject.GetComponent<QuestStep>();
    }
}
