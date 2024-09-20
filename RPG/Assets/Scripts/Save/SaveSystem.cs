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

    public void Save() {
        int slot = variables.GetVariable<int>("slot");
        if (slot < 0 || slot >= maxSlots) return;

        JSONObject obj = new JSONObject();

        obj.AddField("variables", variables.Save());
        obj.AddField("player", Player.instance.Save());
        obj.AddField("inventory", Player.instance.inventario.Save());
        obj.AddField("personagens", dialogaveis.Save());
        obj.AddField("loja", LojaController.instance != null ? LojaController.instance.Save() : null);

        string path = Application.persistentDataPath + "/save_" + slot + ".json";

        Debug.Log("Salvando jogo em: " + path);

        System.IO.File.WriteAllText(path, obj.ToString(true));
    }

    public JSONObject LocalSave() {
        JSONObject obj = new JSONObject();

        obj.AddField("player", Player.instance.Save());
        obj.AddField("inventory", Player.instance.inventario.Save());
        obj.AddField("personagens", dialogaveis.Save());
        obj.AddField("loja", LojaController.instance != null ? LojaController.instance.Save() : null);

        return obj;
    }

    public void LocalLoad(JSONObject obj) {
        if (GameManager.instance.state != GameState.NotStarted) {
            Player.instance.Load(obj.GetField("player"));
            Player.instance.inventario.Load(obj.GetField("inventory"));
            Player.instance.LoadEquipados(obj.GetField("player"));
            dialogaveis.Load(obj.GetField("personagens"));

            if (LojaController.instance != null)
                LojaController.instance.Load(obj.GetField("loja"));

            UIController.equipamentos.UpdateStats();
        }
    }

    public JSONObject Load(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return null;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        if (!System.IO.File.Exists(path)) return null;

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);
        latestLoaded = obj;

        variables.Load(obj.GetField("variables"));

        if (GameManager.instance.state != GameState.NotStarted) {
            if (Player.instance != null && Player.instance.gameObject != null) {
                Player.instance.Load(obj.GetField("player"));
                Player.instance.inventario.Load(obj.GetField("inventory"));
                Player.instance.LoadEquipados(obj.GetField("player"));
            }
            
            dialogaveis.Load(obj.GetField("personagens"));

            if (LojaController.instance != null)
                LojaController.instance.Load(obj.GetField("loja"));
        }
       
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

        if (LojaController.instance != null)
            LojaController.instance.Load(obj.GetField("loja"));

        Debug.Log("Player loaded!");
        return obj;
    }

    public bool HasSave(int slot = 0) {
        if (slot < 0 || slot >= maxSlots) return false;

        string path = Application.persistentDataPath + "/save_" + slot + ".json";
        return System.IO.File.Exists(path);
    }



    // Separei um pedaço do SaveSystem para salvar as Configurações.
    // Apesar de que idealmente o SaveSystem deveria ser uma classe utilizavel em varios contextos,
    // e instanciada multiplas vezes, pelo tempo e prioridades (e considerando o estado que já está),
    // seria muito trabalhoso refatorar o SaveSystem para ser mais genérico.
    // Portanto ele acaba tendo que ser um Singleton

    public void SaveSettings() {
        JSONObject obj = SettingsManager.instance.Save();


        string path = Application.persistentDataPath + "/settings.json";
        System.IO.File.WriteAllText(path, obj.ToString(true));
    }

    public JSONObject LoadSettings() {
        string path = Application.persistentDataPath + "/settings.json";
        if (!System.IO.File.Exists(path)) {
            SettingsManager.instance.LoadDefault();
            return null;
        }

        string json = System.IO.File.ReadAllText(path);
        JSONObject obj = new JSONObject(json);

        SettingsManager.instance.Load(obj);
        return obj;
    }
}