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
}

[System.Serializable]
public class CondicaoParams: AcaoParams {
    public enum Comparacao { NULL, IGUAL, DIFERENTE, MAIOR, MENOR, MAIOR_IGUAL, MENOR_IGUAL}

    public Comparacao comparacaoValue;
    public bool dinamic;

    public static Comparacao GetComparacao(string comparacao) {
        return (Comparacao)System.Enum.Parse(typeof(Comparacao), comparacao);
    }

    public string GetComparacaoString() {
        return comparacaoValue.ToString();
    }

    public string[] GetComparacaoStringArray() {
        return System.Enum.GetNames(typeof(Comparacao));
    }

    public static CondicaoParams Create(string id, string type, string val, string global, string comparacao, string dinamic) {
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