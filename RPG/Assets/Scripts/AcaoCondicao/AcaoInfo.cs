using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class AcaoInfo {
    public AcoesRegistradas acao;
    public AcaoParams parametros;

    public Acao GetAcao() {
        System.Type type = RegistroAcoes.GetRegistro(acao);
        if (type != null) {
            return (Acao)System.Activator.CreateInstance(type, parametros);
        }

        return null;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(AcaoInfo))]
public class AcaoInfoDrawer : PropertyDrawer {
    int quantLines = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        ResetLines();
        SerializedProperty acao = property.FindPropertyRelative("acao");
        SerializedProperty parametros = property.FindPropertyRelative("parametros");

        Rect pos = position;
        pos.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(pos, acao);

        AcoesRegistradas acaoEnum = (AcoesRegistradas) acao.enumValueIndex;
        DrawParametros(acaoEnum, ref pos, parametros);


        EditorGUI.EndProperty();
    }

    void DrawParametros(AcoesRegistradas acao, ref Rect pos, SerializedProperty parametros) {
        if (acao == AcoesRegistradas.NULL) return;

        var tipo = RegistroAcoes.GetRegistro(acao);
        string[] retorno = (string[])tipo.GetMethod("GetParametrosUtilizados").Invoke(null, null);
        string[] traduzido = (string[])tipo.GetMethod("GetParametrosTraduzidos").Invoke(null, null);
        if (retorno.Length == 0) return;

        if (retorno.Length != traduzido.Length) {
            traduzido = retorno;
        }

        EditorGUI.indentLevel++;

        int index = 0;
        foreach (string parametro in retorno) {
            string displayText = traduzido[index];
            DrawParametro(parametros, parametro, displayText, ref pos);
            index++;
        }
        EditorGUI.indentLevel--;
    }

    void DrawParametro(SerializedProperty parametros, string parametro, string displayText, ref Rect pos) {
        if (parametro == "value") {
            DrawValorParametro(parametros, parametro, displayText, ref pos);
            return;
        } else {
            switch (parametro) {
                case "intValue":
                    parametros.FindPropertyRelative("type").enumValueIndex = (int)AcaoParams.Tipo.INT;
                    break;
                case "floatValue":
                    parametros.FindPropertyRelative("type").enumValueIndex = (int)AcaoParams.Tipo.FLOAT;
                    break;
                case "stringValue":
                    parametros.FindPropertyRelative("type").enumValueIndex = (int)AcaoParams.Tipo.STRING;
                    break;
                case "boolValue":
                    parametros.FindPropertyRelative("type").enumValueIndex = (int)AcaoParams.Tipo.BOOL;
                    break;
            }
        }

        SerializedProperty prop = parametros.FindPropertyRelative(parametro);
        NextLine(ref pos);
        EditorGUI.PropertyField(pos, prop, new GUIContent(displayText));
    }

    void DrawValorParametro(SerializedProperty parametros, string parametro, string displayText, ref Rect pos) {
        SerializedProperty prop = parametros.FindPropertyRelative("type");
        NextLine(ref pos);
        EditorGUI.PropertyField(pos, prop, new GUIContent("Tipo:"));

        AcaoParams.Tipo tipo = (AcaoParams.Tipo)prop.enumValueIndex;
        if (tipo == AcaoParams.Tipo.NULL) return;

        string qualValor = "";
        switch (tipo) {
            case AcaoParams.Tipo.INT:
                qualValor = "intValue";
                break;
            case AcaoParams.Tipo.FLOAT:
                qualValor = "floatValue";
                break;
            case AcaoParams.Tipo.STRING:
                qualValor = "stringValue";
                break;
            case AcaoParams.Tipo.BOOL:
                qualValor = "boolValue";
                break;
        }

        prop = parametros.FindPropertyRelative(qualValor);
        NextLine(ref pos);
        EditorGUI.PropertyField(pos, prop, new GUIContent(displayText + ":"));
    }


    void ResetLines() {
        quantLines = 0;
    }

    void NextLine(ref Rect pos) {
        pos.y += EditorGUIUtility.singleLineHeight;
        quantLines++;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * (quantLines + 1);
    }
}
#endif