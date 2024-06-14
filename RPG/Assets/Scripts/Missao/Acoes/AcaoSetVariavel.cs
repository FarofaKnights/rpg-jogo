using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoSetVariavel : Acao {
    public string nomeVariavel;
    public bool isGlobal;
    public CondicaoParams.Tipo tipo;
    public object valor;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "value", "isGlobal" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Variavel", "Valor", "Variavel global" }; }

    public AcaoSetVariavel(string nomeVariavel, bool isGlobal, CondicaoParams.Tipo tipo, object valor) {
        this.nomeVariavel = nomeVariavel;
        this.isGlobal = isGlobal;
        this.tipo = tipo;
        this.valor = valor;
    }

    public AcaoSetVariavel(AcaoParams parametros): base(parametros) {
        nomeVariavel = parametros.id;
        isGlobal = parametros.isGlobal;
        tipo = parametros.type;
        valor = parametros.GetValue();
    }

    public override void Realizar() {
        PrimitiveType primitiveType = GetPrimitiveType();
        string escopo = isGlobal ? "global" : "level";
        SaveSystem.instance.variables.SetVariable(nomeVariavel, primitiveType, valor, escopo);
    }

    PrimitiveType GetPrimitiveType() {
        return tipo switch {
            CondicaoParams.Tipo.INT => PrimitiveType.INT,
            CondicaoParams.Tipo.FLOAT => PrimitiveType.FLOAT,
            CondicaoParams.Tipo.STRING => PrimitiveType.STRING,
            CondicaoParams.Tipo.BOOL => PrimitiveType.BOOL,
            _ => PrimitiveType.FLOAT // Nunca vai acontecer (espero)
        };
    }
}
