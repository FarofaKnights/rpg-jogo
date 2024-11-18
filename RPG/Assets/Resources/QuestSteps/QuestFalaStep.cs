using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class QuestFalaStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public string falaName;
    bool alreadySet = false;
    bool alreadyFinished = false;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;
        Fala[] falas = questInfo.GetFalas(falaName);

        if (falas != null && falas.Length > 0)
            UIController.dialogo.StartDialogo(falas, JustFinish);
        else
            JustFinish();
    }

    void JustFinish() {
        if (alreadyFinished) return;
        alreadyFinished = true;

        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.falaName = parameter;

        Set();
    }

    #if UNITY_EDITOR

    public override string GetEditorName() { return "AÇÃO: Começa dialogo"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string nomeDaFala = stepInfo.step.parameter;
        FalaCarregada fala = null;

        if (nomeDaFala != null && nomeDaFala != "") {
            fala = stepInfo.questInfo.GetFalaCarregada(nomeDaFala);
        }


        int currentFalaCarregada = -1;
        for (int i = 0; i < stepInfo.questInfo.falasCarregadas.Length; i++) {
            if (stepInfo.questInfo.falasCarregadas[i].nome == nomeDaFala) {
                currentFalaCarregada = i;
                break;
            }
        }

        if (currentFalaCarregada > -1) {
            SerializedProperty falaProperty = stepInfo.serializedObject.FindProperty("falasCarregadas").GetArrayElementAtIndex(currentFalaCarregada);
            EditorGUILayout.PropertyField(falaProperty, true);
            stepInfo.serializedObject.ApplyModifiedProperties();
        } else {
            nomeDaFala = EditorGUILayout.TextField("Nome da fala", nomeDaFala);
            if (GUILayout.Button("Criar nova fala") && nomeDaFala != "") {
                FalaCarregada newFalaCarregada = new FalaCarregada();
                newFalaCarregada.nome = nomeDaFala;

                List<FalaCarregada> falasCarregadas = new List<FalaCarregada>(stepInfo.questInfo.falasCarregadas);
                falasCarregadas.Add(newFalaCarregada);
                stepInfo.questInfo.falasCarregadas = falasCarregadas.ToArray();

                stepInfo.serializedObject.Update();
            }
        }

        return nomeDaFala;
    }

    public override void SetEditorParameters(CurrentStepAcaoInfo stepInfo, string parameters) {
        stepInfo.step.parameter = parameters;
    }
    #endif
}
