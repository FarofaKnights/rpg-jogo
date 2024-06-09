using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public interface Saveable {
    JSONObject Save();
    void Load(JSONObject obj);
}

public class SaveSystem {
    public VariableSaveSystem variables = new VariableSaveSystem();

    public void Save() {
        JSONObject obj = new JSONObject();

        obj.AddField("variables", variables.Save());
        obj.AddField("player", Player.instance.Save());

        Debug.Log("Saving!");
        Debug.Log(obj.ToString());

        string path = Application.persistentDataPath + "/save.json";
        System.IO.File.WriteAllText(path, obj.ToString());
    }

    public void Load() {
        string path = Application.persistentDataPath + "/save.json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);

        Debug.Log("Loaded!");
        Debug.Log(obj.ToString());

        variables.Load(obj.GetField("variables"));
        Player.instance.Load(obj.GetField("player"));
    }
}