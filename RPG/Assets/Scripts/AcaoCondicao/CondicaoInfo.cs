using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CondicaoInfo {
    public CondicoesRegistradas condicao;
    public CondicaoParams parametros;

    public Condicao GetCondicao() {
        var tipo = RegistroCondicoes.GetRegistro(condicao);
        if (tipo != null) {
            return (Condicao)System.Activator.CreateInstance(tipo, parametros);
        }
        return null;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CondicaoInfo))]
public class CondicaoInfoDrawer : PropertyDrawer {
    int quantLines = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        ResetLines();

        SerializedProperty condicao = property.FindPropertyRelative("condicao");
        SerializedProperty parametros = property.FindPropertyRelative("parametros");

        Rect pos = position;
        pos.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(pos, condicao);

        CondicoesRegistradas condicaoEnum = (CondicoesRegistradas)condicao.enumValueIndex;
        DrawParametros(condicaoEnum, ref pos, parametros);


        EditorGUI.EndProperty();
    }

    void DrawParametros(CondicoesRegistradas condicao, ref Rect pos, SerializedProperty parametros) {
        if (condicao == CondicoesRegistradas.NULL) return;

        var tipo = RegistroCondicoes.GetRegistro(condicao);
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