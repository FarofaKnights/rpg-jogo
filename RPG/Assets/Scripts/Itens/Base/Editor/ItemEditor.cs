using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[CustomEditor(typeof(Item)), CanEditMultipleObjects]
public class ItemEditor : Editor {
    Item item;

    void OnEnable() {
        item = (Item)target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("data"));
        

        if (item.data == null) {
            GameObject prefab;

            if (item.gameObject.scene.name == null) prefab = item.gameObject;
            else prefab = PrefabUtility.GetCorrespondingObjectFromSource(item.gameObject);
            

            if (GUILayout.Button("Criar novo ItemData")) {
                string name = prefab.name + "Data";
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                string directory = System.IO.Path.GetDirectoryName(prefabPath);
                string path = EditorUtility.SaveFilePanelInProject("Save ItemData", name, "asset", "Save ItemData", directory);
                
                if (path != "") {
                    ItemData data = ScriptableObject.CreateInstance<ItemData>();
                    data.name = System.IO.Path.GetFileNameWithoutExtension(path);
                    AssetDatabase.CreateAsset(data, path);
                    item.data = data;
                    prefab.GetComponent<Item>().data = data;
                    data.prefab = prefab;
                    serializedObject.Update();

                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            GameObject[] instances = PrefabUtility.FindAllInstancesOfPrefab(prefab, SceneManager.GetActiveScene());

            if (instances.Length > 1 || (instances.Length == 1 && instances[0] != item.gameObject)) {
                EditorGUILayout.HelpBox("Há " + instances.Length + " instancias deste prefab na cena atual. Ao criar um novo ItemData, atualize a cena para aplicar as modificações para todas as instâncias!", MessageType.Warning);
            }
            
        } else {

            GUILayout.Space(4);

            

            // Draw ItemData
            GUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Informações do Item (ItemData)", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);

            Editor editor = Editor.CreateEditor(item.data);
            editor.OnInspectorGUI();


            GUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }

} 
