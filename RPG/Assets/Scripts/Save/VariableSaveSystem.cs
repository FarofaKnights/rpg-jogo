using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public enum PrimitiveType { INT, FLOAT, STRING, BOOL }

public class PrimitiveVariable {
    public string name;
    public PrimitiveType type;
    public object value;

    public PrimitiveVariable(string name, PrimitiveType type, object value) {
        this.name = name;
        this.type = type;
        this.value = value;
    }

    public static PrimitiveType GetVariableType<T>() {
        if (typeof(T) == typeof(int)) {
            return PrimitiveType.INT;
        } else if (typeof(T) == typeof(float)) {
            return PrimitiveType.FLOAT;
        } else if (typeof(T) == typeof(string)) {
            return PrimitiveType.STRING;
        } else if (typeof(T) == typeof(bool)) {
            return PrimitiveType.BOOL;
        } else {
            throw new System.Exception("Unsupported type");
        }
    }

}

public class GlobalVariable<T> {
    public string name;
    public T value {
        get {
            return GetValue();
        }
        set {
            SetValue(value);
        }
    }

    public GlobalVariable(string name, T value) {
        this.name = name;
        SetValue(value);
    }

    public GlobalVariable(string name) {
        this.name = name;
    }

    public virtual void SetValue(T value) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        sys.SetVariable(name, type, value);
    }

    public virtual T GetValue() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.GetVariable<T>(name);
    }
}

public class LocalVariable<T>: GlobalVariable<T> {
    public LocalVariable(string name, T value) : base(name, value) { }
    public LocalVariable(string name) : base(name) { }

    public override void SetValue(T value) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        sys.SetVariable(name, type, value, "level");
    }

    public override T GetValue() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.GetVariable<T>(name, "level");
    }
}

public class SaveEscopo {
    Dictionary<string, PrimitiveVariable> variables = new Dictionary<string, PrimitiveVariable>();

    public void SetVariable(string name, PrimitiveType type, object value) {
        if (variables.ContainsKey(name)) {
            variables[name] = new PrimitiveVariable(name, type, value);
        } else {
            variables.Add(name, new PrimitiveVariable(name, type, value));
        }
    }

    public void SetVariable<T>(string name, T value) {
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        SetVariable(name, type, value);
    }

    public T GetVariable<T>(string name) {
        PrimitiveVariable variable = variables[name];
        return (T)variable.value;
    }

    public void ForEach(System.Action<PrimitiveVariable> action) {
        foreach (var variable in variables) {
            action(variable.Value);
        }
    }
}

public class VariableSaveSystem: Saveable {
    SaveEscopo global = new SaveEscopo();
    Dictionary<string, SaveEscopo> escopos = new Dictionary<string, SaveEscopo>();

    public VariableSaveSystem() {
        GameManager.instance.onBeforeSceneChange += SetLevelScope;
        GameManager.instance.onAfterSceneChange += LoadLevelScope;
    }


    public JSONObject Save() {
        JSONObject obj = new JSONObject();
        JSONObject globalObj = SaveEscopo(global);

        if (globalObj.list != null && globalObj.list.Count > 0) {
            obj.AddField("global", globalObj);
        }

        SetLevelScope(GameManager.instance.CurrentSceneName());
        escopos.Remove("level");

        JSONObject levels = new JSONObject();
        int i = 0;
        foreach (var escopo in escopos) {
            JSONObject levelObj = SaveEscopo(escopo.Value);
            if (levelObj.list != null && levelObj.list.Count > 0) {
                levels.AddField(escopo.Key, levelObj);
                i++;
            }
        }

        if (i > 0) obj.AddField("levels", levels);
        else obj.AddField("levels", JSONObject.emptyObject);

        return obj;
    }

    public void Load(JSONObject obj) {
        Clear();
        global = LoadEscopo(obj.GetField("global"));

        JSONObject levels = obj.GetField("levels");
        if (levels != null && levels.list != null) {
            for (int i = 0; i < levels.list.Count; i++) {
                var key = levels.keys[i];
                var value = levels.list[i];
                escopos[key] = LoadEscopo(value);
            }

            LoadLevelScope(GameManager.instance.CurrentSceneName());
        }
    }

    void Clear() {
        global = new SaveEscopo();
        escopos.Clear();
    }

    SaveEscopo LoadEscopo(JSONObject escopoObj) {
        SaveEscopo escopo = new SaveEscopo();

        if (escopoObj == null || escopoObj.list == null) return escopo;

        for (int i = 0; i < escopoObj.list.Count; i++) {
            string name = escopoObj.keys[i];
            JSONObject value = escopoObj.list[i];
            object primitiveValue = null;

            PrimitiveType type = PrimitiveType.INT;

            switch (value.type) {
                case JSONObject.Type.Number:
                    if (value.isInteger) {
                        type = PrimitiveType.INT;
                        primitiveValue = value.intValue;
                    } else {
                        type = PrimitiveType.FLOAT;
                        primitiveValue = value.floatValue;
                    }
                    break;
                case JSONObject.Type.String:
                    type = PrimitiveType.STRING;
                    primitiveValue = value.stringValue;
                    break;
                case JSONObject.Type.Bool:
                    type = PrimitiveType.BOOL;
                    primitiveValue = value.boolValue;
                    break;
            }

            escopo.SetVariable(name, type, primitiveValue);
        }
        return escopo;
    }

    JSONObject SaveEscopo(SaveEscopo escopo) {
        return new JSONObject(escopoObj => {
            escopo.ForEach(variable => {
                switch (variable.type) {
                    case PrimitiveType.INT:
                        escopoObj.AddField(variable.name, (int)variable.value);
                        break;
                    case PrimitiveType.FLOAT:
                        escopoObj.AddField(variable.name, (float)variable.value);
                        break;
                    case PrimitiveType.STRING:
                        escopoObj.AddField(variable.name, (string)variable.value);
                        break;
                    case PrimitiveType.BOOL:
                        escopoObj.AddField(variable.name, (bool)variable.value);
                        break;
                }
            });
        });
    }


    public void SetVariable(string name, PrimitiveType type, object value, string escopo = "global") {
        SaveEscopo esc = GetEscopo(escopo);
        esc.SetVariable(name, type, value);
    }

    public void SetVariable<T>(string name, T value, string escopo = "global") {
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        SetVariable(name, type, value, escopo);
    }

    public T GetVariable<T>(string name, string escopo = "global") {
        SaveEscopo esc = GetEscopo(escopo);
        return esc.GetVariable<T>(name);
    }

    public SaveEscopo GetEscopo(string name) {
        if (name == "global") {
            return global;
        } else if (escopos.ContainsKey(name)) {
            return escopos[name];
        } else {
            SaveEscopo escopo = new SaveEscopo();
            escopos.Add(name, escopo);
            return escopo;
        }
    }

    public bool HasEscopo(string name) {
        return escopos.ContainsKey(name);
    }

    public SaveEscopo GetGlobal() {
        return global;
    }

    public void SetLevelScope(string levelName) {
        SaveEscopo esc = GetEscopo("level");
        escopos[levelName] = esc;
    }

    public void LoadLevelScope(string levelName) {
        SaveEscopo esc = GetEscopo(levelName);
        escopos["level"] = esc;
    }
}
