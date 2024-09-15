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
    public Tipo type;
    public bool isGlobal;
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;

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

    public static AcaoParams Create(string id, string type, string val, string global) {
        AcaoParams acaoParams = new AcaoParams();
        acaoParams.id = id;
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
