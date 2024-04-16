using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ataque))]
public class AtaqueEditor : Editor {
    Ataque ataque;
    GameObject visualizer;
    IAtacador atacador;
    Animator animator;

    void OnEnable() {
        ataque = (Ataque)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI() {
        GameObject oldVisualizer = visualizer;
        // Draw default
        DrawDefaultInspector();

        GUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Visualização da Hitbox", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(4);

            // In scene gameobject field
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Hitbox", GUILayout.Width(146));
                visualizer = (GameObject)EditorGUILayout.ObjectField(visualizer, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            if (oldVisualizer != visualizer) {
                HandleVisualizadorChange();
            }

            // Cria botão de testar animação do visualizador
            if (visualizer != null && animator != null) {
                string attackTriggerName = "Attack";

                // Create a label and input for the attack trigger name
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Nome do Trigger de Ataque", GUILayout.Width(146));
                    attackTriggerName = EditorGUILayout.TextField(attackTriggerName);
                EditorGUILayout.EndHorizontal();

                // Create a button to test the attack animation
                if (GUILayout.Button("Testar Animação de Ataque")) {
                    animator.SetTrigger(attackTriggerName);
                }
            }

        GUILayout.EndVertical();

        
    }

    T GetComponentSomewhere<T>(GameObject go) {
        T component = go.GetComponent<T>();
        if (component != null) return component;

        // Pegar de algum filho
        component = go.GetComponentInChildren<T>();
        if (component != null) return component;

        // Pegar de algum pai
        component = go.GetComponentInParent<T>();
        if (component != null) return component;

        return default(T);
    }

    void HandleVisualizadorChange() {
        if (visualizer != null) {
            atacador = GetComponentSomewhere<IAtacador>(visualizer);

            if (atacador == null) visualizer = null;
            else animator = atacador.GetAnimator();
        }
    }

    public void OnSceneGUI(SceneView sceneView) {
        if (atacador != null) {
            Handles.color = new Color(1, 0, 0, 1);
            
            Vector3 offset = ataque.hitboxOffset;
            Vector3 scale = ataque.hitboxSize;
            Vector3 rotation = ataque.hitboxRotation;

            Vector3 rootPosition = atacador.GetAttackHitboxHolder().transform.position;

            Vector3 position = rootPosition + offset;
            Quaternion rot = visualizer.transform.rotation * Quaternion.Euler(rotation);

            Handles.DrawWireCube(position, scale);
        }
    }


}
