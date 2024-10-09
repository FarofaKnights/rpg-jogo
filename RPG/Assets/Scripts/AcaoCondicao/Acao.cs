using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Acao {
    public static string[] GetParametrosUtilizados(){ return new string[] { }; }
    public static string[] GetParametrosTraduzidos(){ return new string[] { }; }
    
    public Acao() { }
    public Acao(AcaoParams parametros) { }

    public abstract void Realizar();
}

[System.Serializable]
public class AcaoParams {
    public enum Tipo { NULL, INT, FLOAT, STRING, BOOL }

    public string id;
    public string id2; // Um id extra para casos especiais
    public Tipo type;
    public bool isGlobal;
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;

    public AcaoParams() {
        type = Tipo.NULL;
    }

    public AcaoParams(string id, string id2, Tipo type, bool isGlobal, int intValue, float floatValue, string stringValue, bool boolValue) {
        this.id = id;
        this.id2 = id2;
        this.type = type;
        this.isGlobal = isGlobal;
        this.intValue = intValue;
        this.floatValue = floatValue;
        this.stringValue = stringValue;
        this.boolValue = boolValue;
    }

    public AcaoParams(AcaoParams parametros) {
        id = parametros.id;
        id2 = parametros.id2;
        type = parametros.type;
        isGlobal = parametros.isGlobal;
        intValue = parametros.intValue;
        floatValue = parametros.floatValue;
        stringValue = parametros.stringValue;
        boolValue = parametros.boolValue;
    }

    public object GetValue() {
        return type switch {
            Tipo.INT => intValue,
            Tipo.FLOAT => floatValue,
            Tipo.STRING => stringValue,
            Tipo.BOOL => boolValue,
            _ => null
        };
    }

    public Tipo GetTipo() {
        return type;
    }

    public static Tipo GetTipo(string tipo) {
        return (Tipo)System.Enum.Parse(typeof(Tipo), tipo);
    }

    public string GetTipoString() {
        return type.ToString();
    }

    public void SetValue(string value) {
        switch (type) {
            case Tipo.INT:
                intValue = int.Parse(value);
                break;
            case Tipo.FLOAT:
                floatValue = float.Parse(value);
                break;
            case Tipo.STRING:
                stringValue = value;
                break;
            case Tipo.BOOL:
                boolValue = bool.Parse(value);
                break;
        }
    }

    public void SetTipo(string tipo) {
        type = GetTipo(tipo);
    }

    public static AcaoParams Create(string id, string type, string val, string global){
        return Create(id, "", type, val, global);
    }

    public static AcaoParams Create(string id, string id2, string type, string val, string global) {
        AcaoParams acaoParams = new AcaoParams();
        acaoParams.id = id;
        acaoParams.id2 = id2;
        acaoParams.type = GetTipo(type);
        acaoParams.isGlobal = bool.Parse(global);

        switch (acaoParams.type) {
            case Tipo.INT:
                acaoParams.intValue = int.Parse(val);
                break;
            case Tipo.FLOAT:
                acaoParams.floatValue = float.Parse(val);
                break;
            case Tipo.STRING:
                acaoParams.stringValue = val;
                break;
            case Tipo.BOOL:
                acaoParams.boolValue = bool.Parse(val);
                break;
        }

        return acaoParams;
    }
}
