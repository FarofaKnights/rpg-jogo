using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStepPass : MonoBehaviour
{
    public string questVariableName;
    public int step;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            LocalVariable<int> currentStep = new LocalVariable<int>(questVariableName, 0);
            currentStep.value = step;
        }
    }
}
