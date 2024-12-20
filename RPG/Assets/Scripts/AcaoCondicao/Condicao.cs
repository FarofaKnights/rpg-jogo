using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condicao {
    System.Action OnRealizada;
    public bool dinamica = false;
    bool realizada = false;

    public static string[] GetParametrosUtilizados(){ return new string[] { }; }
    public static string[] GetParametrosTraduzidos(){ return new string[] { }; }

    public Condicao() { }
    public Condicao(CondicaoParams parametros) {
        dinamica = parametros.dinamic;
    }

    public abstract bool CheckCondicao();
    public virtual bool Realizar() {
        if (CheckCondicao()) {
            OnRealizada?.Invoke();
            OnRealizada = null;
            realizada = true;
            return true;
        }
        return false;
    }

    public void Then(System.Action action) {
        if (realizada) action();
        else OnRealizada += action;
    }

    public void Clear() {
        OnRealizada = null;
    }
}

[System.Serializable]
public class CondicaoParams: AcaoParams {
    public enum Comparacao { NULL, IGUAL, DIFERENTE, MAIOR, MENOR, MAIOR_IGUAL, MENOR_IGUAL}

    public Comparacao comparacaoValue;
    public bool dinamic;

    public CondicaoParams() {
        type = Tipo.NULL;
        comparacaoValue = Comparacao.NULL;
        dinamic = false;
    }

    public CondicaoParams(string id, string id2, Tipo type, bool isGlobal, int intValue, float floatValue, string stringValue, bool boolValue, Comparacao comparacao, bool dinamic) {
        this.id = id;
        this.id2 = id2;
        this.type = type;
        this.isGlobal = isGlobal;
        this.intValue = intValue;
        this.floatValue = floatValue;
        this.stringValue = stringValue;
        this.boolValue = boolValue;
        this.comparacaoValue = comparacao;
        this.dinamic = dinamic;
    }

    public CondicaoParams(CondicaoParams parametros) {
        id = parametros.id;
        id2 = parametros.id2;
        type = parametros.type;
        isGlobal = parametros.isGlobal;
        intValue = parametros.intValue;
        floatValue = parametros.floatValue;
        stringValue = parametros.stringValue;
        boolValue = parametros.boolValue;
        comparacaoValue = parametros.comparacaoValue;
        dinamic = parametros.dinamic;
    }

    public static Comparacao GetComparacao(string comparacao) {
        // Primeiro checa se comparacao é algum valor do enum Comparacao
        foreach (Comparacao comp in System.Enum.GetValues(typeof(Comparacao))) {
            if (comparacao == comp.ToString()) return comp;
        }
        return Comparacao.NULL;
    }

    public string GetComparacaoString() {
        return comparacaoValue.ToString();
    }

    public string[] GetComparacaoStringArray() {
        return System.Enum.GetNames(typeof(Comparacao));
    }

    public static CondicaoParams Create(string id, string type, string val, string global, string comparacao, string dinamic){
        return Create(id, "", type, val, global, comparacao, dinamic);
    }

    public static CondicaoParams Create(string id, string id2, string type, string val, string global, string comparacao, string dinamic) {
        CondicaoParams condicaoParams = new CondicaoParams();
        condicaoParams.id = id;
        condicaoParams.type = GetTipo(type);
        condicaoParams.isGlobal = bool.Parse(global);
        condicaoParams.comparacaoValue = GetComparacao(comparacao);
        condicaoParams.dinamic = bool.Parse(dinamic);

        switch (condicaoParams.type) {
            case Tipo.INT:
                condicaoParams.intValue = int.Parse(val);
                break;
            case Tipo.FLOAT:
                condicaoParams.floatValue = float.Parse(val);
                break;
            case Tipo.STRING:
                condicaoParams.stringValue = val;
                break;
            case Tipo.BOOL:
                condicaoParams.boolValue = bool.Parse(val);
                break;
        }

        return condicaoParams;
    }
}