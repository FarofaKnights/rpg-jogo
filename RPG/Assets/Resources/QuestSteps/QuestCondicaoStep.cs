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
    Condicao condicaoInstance;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;

        condicaoInstance = condicao.GetCondicao();
        condicaoInstance.Then(FinishStep);
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

    public override void OnEnd() {
        condicaoInstance.Clear();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "CONDIÇÃO: Condição"; }

    public void DrawCondicao(SerializedProperty condicaoProperty) {
        int quantLinhas = condicao.GetQuantidadeParametros();
        if (quantLinhas > 3) quantLinhas += 2;
        else quantLinhas += 1;

        CondicaoInfoDrawer drawer = new CondicaoInfoDrawer();
        GUIContent content = new GUIContent(condicaoProperty.displayName);
        Rect drawerRect = EditorGUILayout.GetControlRect(true, drawer.GetPropertyHeight(condicaoProperty, content) * quantLinhas);
        drawer.OnGUI(drawerRect, condicaoProperty, content);
    }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter ?? "");

        CondicaoInfo condicaoOld = new CondicaoInfo(condicao);
        CondicaoParams oldParams = new CondicaoParams(condicao.parametros);

        if (parameters != null && parameters.Length > 0) {
            string condicaoRegistrada = parameters[0];

            foreach (string condicaoRegName in RegistroCondicoes.GetCondicoesString()) {
                if (condicaoRegName == condicaoRegistrada) {
                    condicao.condicao = RegistroCondicoes.GetCondicao(condicaoRegistrada);
                    break;
                }
            }

            if (parameters.Length > 5) {
                CondicaoParams p = condicao.parametros;
                p.id = parameters[1];
                p.SetTipo(parameters[2]);
                p.SetValue(parameters[3]);
                p.isGlobal = parameters[4] == "True";
                p.comparacaoValue = CondicaoParams.GetComparacao(parameters[5]);
            }
        }
        
        SerializedObject serializedObject = new SerializedObject(this);
        
        SerializedProperty condicaoProperty = serializedObject.FindProperty("condicao");
        DrawCondicao(condicaoProperty);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        

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

        this.condicao = condicaoOld;
        this.condicao.parametros = oldParams;

        return JoinParameters(new string[] { nomeDaCondicao, param_id, param_type, param_value, param_global, param_comparacao });
    }
    #endif
}
