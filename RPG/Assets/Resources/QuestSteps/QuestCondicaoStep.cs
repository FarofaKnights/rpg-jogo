using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestCondicaoStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public CondicaoInfo condicao;
    bool alreadySet = false;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;

        Condicao condicaoInstance = condicao.GetCondicao();
        condicaoInstance.Then(OnConditionRealizada);
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        string condicaoRegistrada = parameters[0];

        foreach (string condRegName in RegistroCondicoes.GetCondicoesString()) {
            if (condRegName == condicaoRegistrada) {
                condicao.condicao = RegistroCondicoes.GetCondicao(condicaoRegistrada);
                break;
            }
        }

        if (parameters.Length > 5) {
            condicao.parametros = CondicaoParams.Create(parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], "True");
            // Vou deixar sempre como dinamico até que seja necessário não ser dinamico
            condicao.parametros.dinamic = true;
        }

        Set();
    }

    void OnConditionRealizada() {
        FinishStep();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "CONDIÇÃO: Condição"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter ?? "");
        
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty condicaoProperty = serializedObject.FindProperty("condicao");
        EditorGUILayout.PropertyField(condicaoProperty, true);

        serializedObject.ApplyModifiedProperties();

        string nomeDaCondicao = RegistroCondicoes.GetCondicaoString(condicao.condicao);
        string param_id = "", param_type = "", param_value = "", param_global = "", param_comparacao = "";
        if (condicao.parametros != null) {
            param_id = condicao.parametros.id;
            param_type = condicao.parametros.GetTipoString();
            param_value = "" + condicao.parametros.GetValue();
            param_global = condicao.parametros.isGlobal ? "True" : "False";
            param_comparacao = condicao.parametros.GetComparacaoString();
            param_comparacao = "True"; // Lembrar de voltar aqui qualquer coisa
        }

        return JoinParameters(new string[] { nomeDaCondicao, param_id, param_type, param_value, param_global, param_comparacao });
    }
    #endif
}
