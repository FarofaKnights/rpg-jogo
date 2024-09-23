#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CurrentStepAcaoInfo {
    public QuestInfo questInfo;
    public GameObjectParameter step;
    public SerializedObject serializedObject;
    public string id;
}

public class NamedStepPrefab {
    public string name;
    public GameObject prefab;
}

[CustomEditor(typeof(QuestInfo))]
public class QuestInfoEditor : Editor {
    QuestInfo questInfo;
    bool useCustomEditor = true;

    GUIStyle textAreaStyle;
    public int padding = 4;

    List<NamedStepPrefab> stepsPrefabs = new List<NamedStepPrefab>();

    public class MoveStep {
        public int index;
        public int direction; // 1 = down, -1 = up
    }

    public void OnEnable() {
        questInfo = (QuestInfo)target;

        // Get all the steps prefabs
        stepsPrefabs.Clear();
        Object[] objects = Resources.LoadAll("QuestSteps", typeof(GameObject));
        foreach (Object obj in objects) {
            NamedStepPrefab namedStepPrefab = new NamedStepPrefab();
            namedStepPrefab.name = obj.name;
            namedStepPrefab.prefab = (GameObject)obj;
            stepsPrefabs.Add(namedStepPrefab);
        }
    }

    public override void OnInspectorGUI() {
        useCustomEditor = EditorGUILayout.Toggle("Use Custom Editor", useCustomEditor);

        if (useCustomEditor) DrawCustomInspector();
        else DrawDefaultInspector();
    }

    public void DrawCustomInspector() {
        EditorGUI.BeginChangeCheck();

        GUI.backgroundColor = Color.white;

        textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;

        questInfo.titulo = EditorGUILayout.TextField("Titulo", questInfo.titulo);

        EditorGUILayout.LabelField("Descrição:");
        questInfo.descricao = EditorGUILayout.TextArea(questInfo.descricao, textAreaStyle, GUILayout.Height(60));

        DrawCondicao();
        GUILayout.Space(padding);
        DrawStepsHolder();

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(questInfo);
        }
    }

    public void DrawCondicao() {
        SerializedProperty requirementsInfo = serializedObject.FindProperty("requirementsInfo");
        EditorGUILayout.PropertyField(requirementsInfo, true);
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawStepsHolder() {
        if (questInfo.steps == null) {
            questInfo.steps = new GameObjectParameterParent[0];
        }

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Passos:");
        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) {
            List<GameObjectParameterParent> list = new List<GameObjectParameterParent>(questInfo.steps);
            list.Add(new GameObjectParameterParent());
            questInfo.steps = list.ToArray();
        }

        GUILayout.EndHorizontal();


        GUILayout.BeginVertical("box");
        GUILayout.Space(padding);

        MoveStep actuallMoveInfo = null;
        int counter = 0;
        foreach (GameObjectParameter step in questInfo.steps) {
            MoveStep moveInfo;
            GameObjectParameter res = DrawStep(step, counter, questInfo.steps.Length, false, out moveInfo);
            questInfo.steps[counter] = (GameObjectParameterParent)res;
            if (moveInfo != null) actuallMoveInfo = moveInfo;
    
            counter++;
        }

        if (actuallMoveInfo != null) {
            List<GameObjectParameterParent> list = new List<GameObjectParameterParent>(questInfo.steps);
            GameObjectParameterParent temp = list[actuallMoveInfo.index];
            list[actuallMoveInfo.index] = list[actuallMoveInfo.index + actuallMoveInfo.direction];
            list[actuallMoveInfo.index + actuallMoveInfo.direction] = temp;
            questInfo.steps = list.ToArray();
        }

        for (int i = questInfo.steps.Length - 1; i >= 0; i--) {
            if (questInfo.steps[i] == null) {
                List<GameObjectParameterParent> list = new List<GameObjectParameterParent>(questInfo.steps);
                list.RemoveAt(i);
                questInfo.steps = list.ToArray();
            }
        }

        GUILayout.Space(padding);
        GUILayout.EndVertical();
    }

    public GameObjectParameter DrawStepHeader(GameObjectParameter step, string title, int i, int size, out MoveStep moveStep, int j = -1) {
        GUILayout.BeginHorizontal();
        moveStep = null;

        // Show / Hide
        string character = step.mostrandoConteudo ? "▼" : "▶";
        if (GUILayout.Button(character, GUILayout.Width(20), GUILayout.Height(20))) {
            step.mostrandoConteudo = !step.mostrandoConteudo;
        }

        if (j > -1) title += " " + j + "." + i;
        else  title += " " + i;

        if (step.informativo != "") title += " - " + step.informativo;
        else if (step.type == QuestStepType.PAI) title += " - PAI [" + ((GameObjectParameterParent)step).children.Length + "]";
        else if (step.gameObject != null) title += " - " + step.gameObject.GetComponent<QuestStep>().GetEditorName() + (step.parameter != "" ? " [" + step.parameter + "]" : "");
        else title += " - ACAO";


        title += step.mostrandoConteudo ? ":" : "";
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        // Move
        if (i > 0 && GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(20))) {
            moveStep = new MoveStep();
            moveStep.index = i;
            moveStep.direction = -1;
        }

        if (i < size - 1 && GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(20))) {
            moveStep = new MoveStep();
            moveStep.index = i;
            moveStep.direction = 1;
        }


        // Delete
        if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) {
            return null;
        }

        GUILayout.EndHorizontal();

        return step;
    }

    public GameObjectParameter DrawSelectPrefab(GameObjectParameter step) {
        GameObject selected = step.gameObject;
        int index = 0;

        string[] options = new string[stepsPrefabs.Count + 1];
        options[0] = "Nenhum selecionado";

        for (int i = 0; i < stepsPrefabs.Count; i++) {
            options[i + 1] = stepsPrefabs[i].prefab.GetComponent<QuestStep>().GetEditorName();

            if (stepsPrefabs[i].prefab == selected) {
                index = i + 1;
            }
        }
        int indexOld = index;
        index = EditorGUILayout.Popup("Ação", index, options);

        if (index == 0) {
            selected = null;
            selected = (GameObject)EditorGUILayout.ObjectField("Ação customizada", selected, typeof(GameObject), true);
        } else if (index <= stepsPrefabs.Count) {
            selected = stepsPrefabs[index - 1].prefab;
        }

        if (index != indexOld) {
            serializedObject.Update();
        }

        step.gameObject = selected;
        return step;
    }

    public GameObjectParameter DrawStep(GameObjectParameter step, int i, int size, bool isChild, out MoveStep moveStep, int j = -1) {
        moveStep = null;
        if (step == null) return null;

        GUI.backgroundColor = step.type == QuestStepType.ACAO ? Color.magenta : Color.cyan;

        GUILayout.BeginVertical("box");
        GUILayout.Space(padding);

        step = DrawStepHeader(step, "Passo", i, size, out moveStep, j);

        if (step != null && step.mostrandoConteudo) {
            step.informativo = EditorGUILayout.TextField("Informativo", step.informativo);
            
            if (isChild) step.type = QuestStepType.ACAO;
            else step.type = (QuestStepType)EditorGUILayout.EnumPopup("Tipo", step.type);


            if (step.type == QuestStepType.ACAO) {
                DrawSelectPrefab(step);

                if (step.gameObject != null) {
                    QuestStep questStep = step.gameObject.GetComponent<QuestStep>();
                    CurrentStepAcaoInfo infos = new CurrentStepAcaoInfo();
                    infos.questInfo = questInfo;
                    infos.step = step;
                    infos.serializedObject = serializedObject;
                    infos.id = i + "." + j;
                    
                    string pars = questStep.GetEditorParameters(infos);
                    questStep.SetEditorParameters(infos, pars);
                }

            } else {
                GameObjectParameterParent parent = (GameObjectParameterParent)step;
                parent.children = DrawStepChildren(parent.children, i);
            }
        }

        GUILayout.Space(padding);
        GUILayout.EndVertical();

        return step;
    }

    public GameObjectParameter[] DrawStepChildren(GameObjectParameter[] children, int i) {
        GUI.backgroundColor = Color.white;
        GUILayout.Space(padding);

        GUILayout.BeginVertical("box");
        GUILayout.Space(padding);



        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filhos:", EditorStyles.boldLabel);

        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) {
            List<GameObjectParameter> list = new List<GameObjectParameter>(children);
            list.Add(new GameObjectParameter());
            children = list.ToArray();
        }

        GUILayout.EndHorizontal();



        if (children == null) {
            children = new GameObjectParameter[0];
        }

        int childToDelete = -1;
        int counter = 0;

        MoveStep actuallMoveInfo = null;

        foreach (GameObjectParameter child in children) {
            MoveStep moveStep;
            GameObjectParameter res = DrawStep(child, counter, children.Length, true, out moveStep, i);
            if (res==null) childToDelete = counter;
            if (moveStep != null) {
                actuallMoveInfo = moveStep;
            }

            counter++;
        }

        if (childToDelete != -1) {
            List<GameObjectParameter> list = new List<GameObjectParameter>(children);
            GameObjectParameter temp = list[childToDelete];

            if (temp.gameObject != null) {
                temp.gameObject.GetComponent<QuestStep>().OnStepDelete(new CurrentStepAcaoInfo() {
                    questInfo = questInfo,
                    step = temp,
                    serializedObject = serializedObject
                });
            }

            list.RemoveAt(childToDelete);
            children = list.ToArray();
        }

        if (actuallMoveInfo != null) {
            List<GameObjectParameter> list = new List<GameObjectParameter>(children);
            GameObjectParameter temp = list[actuallMoveInfo.index];
            list[actuallMoveInfo.index] = list[actuallMoveInfo.index + actuallMoveInfo.direction];
            list[actuallMoveInfo.index + actuallMoveInfo.direction] = temp;
            children = list.ToArray();
        }

        GUILayout.Space(padding);
        GUILayout.EndVertical();
        
        return children;
    }

}

#endif