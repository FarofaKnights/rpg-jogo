using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestAcaoStep : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public AcaoInfo acao;
    bool alreadySet = false;

    void Start() {
        Set();
    }

    void Set() {
        if (alreadySet) return;

        alreadySet = true;

        Acao acaoInstance = acao.GetAcao();
        acaoInstance.Realizar();

        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;

        string[] parameters = SeparateParameters(parameter);

        string acaoRegistrada = parameters[0];

        foreach (string acaoRegName in RegistroAcoes.GetAcoesString()) {
            if (acaoRegName == acaoRegistrada) {
                acao.acao = RegistroAcoes.GetAcao(acaoRegistrada);
                break;
            }
        }

        if (parameters.Length > 4) {
            acao.parametros = AcaoParams.Create(parameters[1], parameters[2], parameters[3], parameters[4]);
        }

        Set();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Ação"; }

    public void DrawAcao(SerializedProperty acaoProperty) {
        int quantLinhas = acao.GetQuantidadeParametros();
        if (quantLinhas > 2) quantLinhas += 2;
        else quantLinhas += 1;

        AcaoInfoDrawer drawer = new AcaoInfoDrawer();
        GUIContent content = new GUIContent(acaoProperty.displayName);
        Rect drawerRect = EditorGUILayout.GetControlRect(true, drawer.GetPropertyHeight(acaoProperty, content) * quantLinhas);
        drawer.OnGUI(drawerRect, acaoProperty, content);
    }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter ?? "");
        
        AcaoInfo acaoOld = new AcaoInfo(acao);
        AcaoParams oldParams = new AcaoParams(acao.parametros);

        if (parameters != null && parameters.Length > 0) {
            string acaoRegistrada = parameters[0];

            foreach (string acaoRegName in RegistroAcoes.GetAcoesString()) {
                if (acaoRegName == acaoRegistrada) {
                    acao.acao = RegistroAcoes.GetAcao(acaoRegistrada);
                    break;
                }
            }

            if (parameters.Length > 4) {
                AcaoParams p = acao.parametros;
                p.id = parameters[1];
                p.SetTipo(parameters[2]);
                p.SetValue(parameters[3]);
                p.isGlobal = parameters[4] == "True";
            }
        }


        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty acaoProperty = serializedObject.FindProperty("acao");
        DrawAcao(acaoProperty);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();


        string nomeDaAcao = RegistroAcoes.GetAcaoString(acao.acao);

        string param_id = "", param_type = "", param_value = "", param_global = "";
        if (acao.parametros != null) {
            param_id = acao.parametros.id;
            param_type = acao.parametros.GetTipoString();
            param_value = "" + acao.parametros.GetValue();
            param_global = acao.parametros.isGlobal ? "True" : "False";
        }

        this.acao = acaoOld;
        this.acao.parametros = oldParams;

        return JoinParameters(new string[] { nomeDaAcao, param_id, param_type, param_value, param_global });
    }
    #endif
}
