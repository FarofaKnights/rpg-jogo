using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStep {
    public int step;
    public GameObject[] toActivate;
    public GameObject[] toDeactivate;
}

public class QuestController : MonoBehaviour {
    public string questVariableName;

    [SerializeField]
    public QuestStep[] steps;
    LocalVariable<int> currentStep;

    void Start() {
        currentStep = new LocalVariable<int>(questVariableName, 0);
        currentStep.OnChange(OnStepChange);

        for (int i = 0; i <= currentStep.value; i++) {
            OnStepChange(i);
        }
    }

    public void OnStepChange(object stepObj) {
        Debug.Log("Step change: " + stepObj);

        int step = (int)stepObj;
        if (step < 0) return;

        QuestStep questStep = GetStep(step);
        if (questStep == null) return;

        foreach (GameObject go in questStep.toActivate) {
            go.SetActive(true);
        }

        foreach (GameObject go in questStep.toDeactivate) {
            go.SetActive(false);
        }
    }

    void OnDestroy() {
        currentStep.Unwatch(OnStepChange);
    }

    QuestStep GetStep(int step) {
        foreach (QuestStep questStep in steps) {
            if (questStep.step == step) {
                return questStep;
            }
        }

        return null;
    }

}
