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
            bool dontBother = false;
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(item.gameObject);

            if (prefab == null) {
                prefab = item.gameObject;
                dontBother = true;
            }
            

            if (GUILayout.Button("Criar novo ItemData")) {
                string name = prefab.name + "Data";
                string prefabPath = AssetDatabase.GetAssetPath(prefab);

                Debug.Log(prefabPath);
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

            if (SceneManager.GetActiveScene() != null && !dontBother) {
                GameObject[] instances = PrefabUtility.FindAllInstancesOfPrefab(prefab, SceneManager.GetActiveScene());

                if (instances.Length > 1 || (instances.Length == 1 && instances[0] != item.gameObject)) {
                    EditorGUILayout.HelpBox("Há " + instances.Length + " instancias deste prefab na cena atual. Ao criar um novo ItemData, atualize a cena para aplicar as modificações para todas as instâncias!", MessageType.Warning);
                }
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

            if (GUILayout.Button("Setar como item dropado (não tem como voltar atrás)")) {
                GameObject obj = item.gameObject;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefab == null) {
                    prefab = obj;
                }

                ItemDropado dropInfo = obj.AddComponent<ItemDropado>();
                dropInfo.item = prefab.GetComponent<Item>();

                obj.tag = "Item";
                BoxCollider collider = obj.AddComponent<BoxCollider>();
                collider.isTrigger = true;

                DestroyImmediate(obj.GetComponent<TipoAbstrato>());
                DestroyImmediate(item);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

} 
