using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoEditVariavel : Acao {
    public string nomeVariavel;
    public PrimitiveOperations operacao;
    public bool isGlobal;
    public CondicaoParams.Tipo tipo;
    public object valor;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "operacao", "value", "isGlobal" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Variavel", "Operacao", "Variavel global" }; }

    public AcaoEditVariavel(string nomeVariavel, PrimitiveOperations operacao, bool isGlobal, CondicaoParams.Tipo tipo, object valor) {
        this.nomeVariavel = nomeVariavel;
        this.operacao = operacao;
        this.isGlobal = isGlobal;
        this.tipo = tipo;
        this.valor = valor;
    }

    public AcaoEditVariavel(AcaoParams parametros): base(parametros) {
        nomeVariavel = parametros.id;
        operacao = parametros.operacao;
        isGlobal = parametros.isGlobal;
        tipo = parametros.type;
        valor = parametros.GetValue();
    }

    public override void Realizar() {
        PrimitiveType primitiveType = GetPrimitiveType();
        string escopo = isGlobal ? "global" : "level";

        SaveEscopo escopoObj =  SaveSystem.instance.variables.GetEscopo(escopo);
        if (!escopoObj.HasVariable(nomeVariavel)) {
            object defaultValue = PrimitiveVariable.GetDefaultValue(primitiveType);
            escopoObj.SetVariable(nomeVariavel, primitiveType, defaultValue);
        }

        escopoObj.Operacao(nomeVariavel, operacao, valor);
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
