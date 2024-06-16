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
}