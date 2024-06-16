using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public class DialogoSystem: Saveable {
    public JSONObject latestLoaded;

    public JSONObject Save() {
        JSONObject obj = latestLoaded ?? new JSONObject();

        // Find all Dialogavel in the scene
        Dialogavel[] dialogaveis = GameManager.instance.GetObjectsOfType<Dialogavel>();
        foreach (Dialogavel dialogavel in dialogaveis) {
            obj.RemoveField(dialogavel.nome);
            obj.AddField(dialogavel.nome, dialogavel.Save());
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        latestLoaded = obj;

        // Find all Dialogavel in the scene
        Dialogavel[] dialogaveis = GameManager.instance.GetObjectsOfType<Dialogavel>();
        foreach (Dialogavel dialogavel in dialogaveis) {
            dialogavel.Load(obj.GetField(dialogavel.nome));
        }
    }
}
