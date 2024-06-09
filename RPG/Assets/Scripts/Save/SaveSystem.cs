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
    public int maxSlots = 3;

    public void Save(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return;

        JSONObject obj = new JSONObject();

        obj.AddField("variables", variables.Save());
        obj.AddField("player", Player.instance.Save());
        obj.AddField("inventory", Player.instance.inventario.Save());

        string path = Application.persistentDataPath + "/save_" + slot + ".json";

        Debug.Log("Salvando jogo em: " + path);

        System.IO.File.WriteAllText(path, obj.ToString(true));
    }

    public void Load(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);

        variables.Load(obj.GetField("variables"));
        Player.instance.Load(obj.GetField("player"));
        Player.instance.inventario.Load(obj.GetField("inventory"));
        Player.instance.LoadEquipados(obj.GetField("player"));

        Debug.Log("Loaded!");
    }

    public void LoadPlayer(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);

        Player.instance.Load(obj.GetField("player"));
        Player.instance.inventario.Load(obj.GetField("inventory"));
        Player.instance.LoadEquipados(obj.GetField("player"));

        Debug.Log("Player loaded!");
    }
}