using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Acao {
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
}
