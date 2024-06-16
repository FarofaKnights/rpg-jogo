using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public interface Saveable {
    JSONObject Save();
    void Load(JSONObject obj);
}

public class SaveSystem {
    public static SaveSystem instance { get{ return GameManager.instance.save; }}
    public VariableSaveSystem variables = new VariableSaveSystem();
    public DialogoSystem dialogaveis = new DialogoSystem();

    public int maxSlots = 3;
    public JSONObject latestLoaded;

    public string GetSaveFolder() {
        return Application.persistentDataPath;
    }

    public void Save(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return;

        JSONObject obj = new JSONObject();

        obj.AddField("variables", variables.Save());
        obj.AddField("player", Player.instance.Save());
        obj.AddField("inventory", Player.instance.inventario.Save());
        obj.AddField("personagens", dialogaveis.Save());

        string path = Application.persistentDataPath + "/save_" + slot + ".json";

        Debug.Log("Salvando jogo em: " + path);

        System.IO.File.WriteAllText(path, obj.ToString(true));
    }

    public JSONObject Load(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return null;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        if (!System.IO.File.Exists(path)) return null;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);
        latestLoaded = obj;

        variables.Load(obj.GetField("variables"));
        Player.instance.Load(obj.GetField("player"));
        Player.instance.inventario.Load(obj.GetField("inventory"));
        Player.instance.LoadEquipados(obj.GetField("player"));
        dialogaveis.Load(obj.GetField("personagens"));

        Debug.Log("Loaded!");
        return obj;
    }

    public JSONObject LoadPlayer(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return null;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        if (!System.IO.File.Exists(path)) return null;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);
        latestLoaded = obj;
        
        Player.instance.Load(obj.GetField("player"));
        Player.instance.inventario.Load(obj.GetField("inventory"));
        Player.instance.LoadEquipados(obj.GetField("player"));
        dialogaveis.Load(obj.GetField("personagens"));

        Debug.Log("Player loaded!");
        return obj;
    }
}