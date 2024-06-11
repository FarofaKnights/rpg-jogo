using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condicao {
    public System.Action OnRealizada;
    public bool dinamica = false;

    public Condicao() { }
    public Condicao(CondicaoParams parametros) {
        dinamica = parametros.dinamic;
    }

    public abstract bool CheckCondicao();
    public void Realizar() {
        if (CheckCondicao()) {
            OnRealizada?.Invoke();
            OnRealizada = null;
        }
    }
}

[System.Serializable]
public class CondicaoParams: AcaoParams {
    public enum Comparacao { NULL, IGUAL, DIFERENTE, MAIOR, MENOR, MAIOR_IGUAL, MENOR_IGUAL}

    public Comparacao comparacaoValue;
    public bool dinamic;
}