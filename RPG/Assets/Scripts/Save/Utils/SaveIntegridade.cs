using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveIntegridade : MonoBehaviour {
    public string id = "";
    LocalVariable<bool> vivo;

    bool autoDestroy = false;
    bool isWatching = false;

    void Start() {
        if (this.id == "" ) {
            // Debug.LogWarning("SalvarIntegridade sem id, geramos um automaticamente para você, mas é recomendado que você defina um id único para cada objeto com este componente.");
            AutoId<SaveIntegridade>(this, gameObject.name);
        }

        vivo = new LocalVariable<bool>("$integridade_" + id, true);
        if (!vivo.value) {
            autoDestroy = true;
            Destroy(gameObject);
        } else if (vivo.isDefault) {
            isWatching = true;
            vivo.OnChange(ReConferir);
        }
    }

    void ReConferir(object value) {
        if (gameObject == null) return;

        if ((bool) value) {
            autoDestroy = true;
            Destroy(gameObject);

            if (isWatching) {
                vivo.Unwatch(ReConferir);
                isWatching = false;
            }
        }
    }

    void OnDestroy() {
        if (!autoDestroy && gameObject.scene.isLoaded) {
            vivo.value = false;
        }

        if (isWatching) vivo.Unwatch(ReConferir);
    }

    public static void AutoId<T>(T obj, string identificadorComum) where T : SaveIntegridade {
        // Tentar definir um id unico com o formato: "%auto_id_" + identificadorComum + "_" + autoId

        T[] itens = FindObjectsOfType<T>();
        string autoIdStart = "%auto_id_" + identificadorComum + "_";

        int lastId = -1;

        foreach (T item in itens) {
            if (item == obj) break;
            if (!item.id.StartsWith(autoIdStart)) break;

            int num = 0;
            string numStr = item.id.Substring(autoIdStart.Length);
            if (int.TryParse(numStr, out num)) {
                if (num > lastId) {
                    lastId = num;
                }
            }
        }

        lastId++;
        obj.id = autoIdStart + lastId;
    }
}
