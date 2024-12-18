using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public enum PrimitiveType { INT, FLOAT, STRING, BOOL }
public enum PrimitiveOperations { NULL, IGUAL, SOMA, SUBTRACAO }
public enum PrimitiveConditions { NULL, IGUAL, DIFERENTE, MAIOR, MENOR }

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

    public static PrimitiveType GetVariableType(string type) {
        switch (type) {
            case "int":
                return PrimitiveType.INT;
            case "float":
                return PrimitiveType.FLOAT;
            case "string":
                return PrimitiveType.STRING;
            case "bool":
                return PrimitiveType.BOOL;
            default:
                throw new System.Exception("Unsupported type");
        }
    }

    public static object GetDefaultValue(PrimitiveType type) {
        switch (type) {
            case PrimitiveType.INT:
                return 0;
            case PrimitiveType.FLOAT:
                return 0.0f;
            case PrimitiveType.STRING:
                return "";
            case PrimitiveType.BOOL:
                return false;
            default:
                throw new System.Exception("Unsupported type");
        }
    }

    public override string ToString() {
        return name + ": " + value.ToString();
    }

    public void Operacao(PrimitiveOperations operacao, object valor) {
        switch (operacao) {
            case PrimitiveOperations.IGUAL:
                value = valor;
                break;
            case PrimitiveOperations.SOMA:
                Somar(valor);
                break;
            case PrimitiveOperations.SUBTRACAO:
                Subtrair(valor);
                break;
        }
    }

    public void Somar(object valor) {
        switch (type) {
            case PrimitiveType.INT:
                value = (int)value + (int)valor;
                break;
            case PrimitiveType.FLOAT:
                value = (float)value + (float)valor;
                break;
            case PrimitiveType.STRING:
                value = (string)value + (string)valor;
                break;
            case PrimitiveType.BOOL:
                value = (bool)value || (bool)valor;
                break;
        }
    }

    public void Subtrair(object valor) {
        switch (type) {
            case PrimitiveType.INT:
                value = (int)value - (int)valor;
                break;
            case PrimitiveType.FLOAT:
                value = (float)value - (float)valor;
                break;
            case PrimitiveType.STRING:
                throw new System.Exception("Can't subtract strings");
            case PrimitiveType.BOOL:
                value = (bool)value && !(bool)valor;
                break;
        }
    }
}

public abstract class ISaveVariable<T> {
    public string name;
    public bool isDefault;
    public T value {
        get {
            return GetValue();
        }
        set {
            SetValue(value);
        }
    }

    public abstract void SetValue(T value);
    public abstract T GetValue();
    public abstract bool Exists();


    public abstract void OnChange(System.Action<object> action);
    public abstract void Unwatch(System.Action<object> action);
}

public class GlobalVariable<T>: ISaveVariable<T> {
    public GlobalVariable(string name, T defaultValue) {
        this.name = name;
        if (!Exists()) {
            SetValue(defaultValue);
            isDefault = true;
        }
    }

    public GlobalVariable(string name) {
        this.name = name;
    }

    public override void SetValue(T value) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        isDefault = false;
        sys.SetVariable(name, type, value);
    }

    public override T GetValue() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.GetVariable<T>(name);
    }

    public override bool Exists() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.GetGlobal().HasVariable(name);
    }

    public override void OnChange(System.Action<object> action) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        sys.GetGlobal().Watch(name, action);
    }

    public override void Unwatch(System.Action<object> action) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        sys.GetGlobal().Unwatch(name, action);
    }
}

public class LocalVariable<T>: ISaveVariable<T> {
    public bool isDefault;

    public LocalVariable(string name, T defaultValue) {
        this.name = name;
        if (!Exists()) {
            SetValue(defaultValue);
            isDefault = true;
        }
    }

    public LocalVariable(string name) {
        this.name = name;
    }

    public override void SetValue(T value) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        PrimitiveType type = PrimitiveVariable.GetVariableType<T>();
        isDefault = false;
        sys.SetVariable(name, type, value, "level");
    }

    public override T GetValue() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.GetVariable<T>(name, "level");
    }

    public override bool Exists() {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        return sys.HasEscopo("level") && sys.GetEscopo("level").HasVariable(name);
    }

    public override void OnChange(System.Action<object> action) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        sys.GetEscopo("level").Watch(name, action);
    }

    public override void Unwatch(System.Action<object> action) {
        VariableSaveSystem sys = GameManager.instance.save.variables;
        sys.GetEscopo("level").Unwatch(name, action);
    }
}

public class SaveEscopo {
    Dictionary<string, PrimitiveVariable> variables = new Dictionary<string, PrimitiveVariable>();
    Dictionary<string, System.Action<object>> watchers = new Dictionary<string, System.Action<object>>();

    public void SetVariable(string name, PrimitiveType type, object value) {
        if (variables.ContainsKey(name)) {
            variables[name] = new PrimitiveVariable(name, type, value);
        } else {
            variables.Add(name, new PrimitiveVariable(name, type, value));
        }

        string currentLevel = GameManager.instance.CurrentSceneName();

        if (watchers.ContainsKey(name) && watchers[name] != null) {
            watchers[name](value);
        } else if (currentLevel == name && watchers.ContainsKey("level") && watchers["level"] != null) {
            watchers["level"](value);
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

    public void Operacao(string name, PrimitiveOperations operacao, object valor) {
        if (variables.ContainsKey(name)) {
            variables[name].Operacao(operacao, valor);
        }
    }

    public void Somar(string name, object valor) {
        if (variables.ContainsKey(name)) {
            variables[name].Somar(valor);
        }
    }

    public void Subtrair(string name, object valor) {
        if (variables.ContainsKey(name)) {
            variables[name].Subtrair(valor);
        }
    }

    public void ForEach(System.Action<PrimitiveVariable> action) {
        foreach (var variable in variables) {
            action(variable.Value);
        }
    }

    public bool HasVariable(string name) {
        return variables.ContainsKey(name);
    }

    public void Watch(string name, System.Action<object> action) {
        if (watchers.ContainsKey(name)) {
            watchers[name] += action;
        } else {
            watchers.Add(name, action);
        }
    }

    public void Unwatch(string name, System.Action<object> action) {
        if (watchers.ContainsKey(name)) {
            watchers[name] -= action;
        }
    }

    public string[] GetVariables() {
        List<string> keys = new List<string>();
        foreach (PrimitiveVariable variable in variables.Values) {
            keys.Add(variable.ToString());
        }

        return keys.ToArray();
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

        LoadLevelScope(GameManager.instance.CurrentSceneName());

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

    public void Clear() {
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

    public bool HasVariable(string name, string escopo = "global") {
        SaveEscopo esc = GetEscopo(escopo);
        return esc.HasVariable(name);
    }

    public void Watch(string name, System.Action<object> action, string escopo = "global") {
        SaveEscopo esc = GetEscopo(escopo);
        esc.Watch(name, action);
    }

    public void Unwatch(string name, System.Action<object> action, string escopo = "global") {
        SaveEscopo esc = GetEscopo(escopo);
        esc.Unwatch(name, action);
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

    public string[] GetEscopos() {
        List<string> keys = new List<string>();
        keys.Add("global");
        foreach (var key in escopos.Keys) {
            keys.Add(key);
        }
        return keys.ToArray();
    }

    public string[] GetTipos() {
        var values = System.Enum.GetValues(typeof(AcaoParams.Tipo));
        string[] names = new string[values.Length];
        for (int i = 0; i < values.Length; i++) {
            names[i] = values.GetValue(i).ToString();
        }
        return names;
    }
}
