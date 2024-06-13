using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CondicoesRegistradas {
    NULL,
    SE_VARIAVEL,
    SE_TEM_ITEM
}


public class RegistroCondicoes {
    public static Dictionary<CondicoesRegistradas, System.Type> condicoesRegistradas = new Dictionary<CondicoesRegistradas, System.Type>() {
        { CondicoesRegistradas.SE_VARIAVEL, typeof(CondicaoIfVariavel) },
        { CondicoesRegistradas.SE_TEM_ITEM, typeof(CondicaoTemItem) }
    };


    public static System.Type GetRegistro(CondicoesRegistradas condicao) {
        if (condicoesRegistradas.ContainsKey(condicao)) {
            return condicoesRegistradas[condicao];
        }

        return null;
    }
}
