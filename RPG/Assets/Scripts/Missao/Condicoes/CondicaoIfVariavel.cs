using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondicaoIfVariavel : Condicao {
    public string nomeVariavel;
    public bool isGlobal;
    public CondicaoParams.Comparacao comparacao;
    public CondicaoParams.Tipo tipo;
    public object valor;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "comparacaoValue", "value", "isGlobal", "dinamic" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Variavel", "Comparação", "Valor", "Variavel global", "É dinâmico" }; }



    public CondicaoIfVariavel(string nomeVariavel, bool isGlobal, CondicaoParams.Comparacao comparacao, CondicaoParams.Tipo tipo, object valor, bool dinamica) {
        this.nomeVariavel = nomeVariavel;
        this.isGlobal = isGlobal;
        this.comparacao = comparacao;
        this.tipo = tipo;
        this.valor = valor;
        this.dinamica = dinamica;

        if (this.dinamica)
            DefineEvent();
    }

    public CondicaoIfVariavel(CondicaoParams parameters): base(parameters) {
        this.nomeVariavel = parameters.id;
        this.isGlobal = parameters.isGlobal;
        this.comparacao = parameters.comparacaoValue;
        this.tipo = parameters.type;
        this.valor = parameters.GetValue();

        if (this.dinamica)
            DefineEvent();
    }

    protected void DefineEvent() {
        Realizar(); // Tenta realizar a condição logo de cara
        
        // Você pensaria que tem um jeito melhor de fazer isso... mas não tem (até onde percebi)
        switch (tipo) {
            case CondicaoParams.Tipo.INT:
                SetarEvento<int>();
                break;
            case CondicaoParams.Tipo.FLOAT:
                SetarEvento<float>();
                break;
            case CondicaoParams.Tipo.STRING:
                SetarEvento<string>();
                break;
            case CondicaoParams.Tipo.BOOL:
                SetarEvento<bool>();
                break;
        }
    }

    void SetarEvento<T>() {
        ISaveVariable<T> variavel = GetVariable<T>();
        variavel.OnChange(Realizar);
        this.Then(() => {
            variavel.Unwatch(Realizar);
        });
    }

    protected void Realizar(object value) {
        Realizar();
    }

    public override bool CheckCondicao() {
        if (!SaveSystem.instance.variables.HasVariable(nomeVariavel, (isGlobal? "global": "level"))) return false;
        
        switch (tipo) {
            case CondicaoParams.Tipo.INT:
                return CheckCondicao((int)valor, (int)GetVariable<int>().value);
            case CondicaoParams.Tipo.FLOAT:
                return CheckCondicao((float)valor, (float)GetVariable<float>().value);
            case CondicaoParams.Tipo.STRING:
                return CheckCondicao((string)valor, (string)GetVariable<string>().value);
            case CondicaoParams.Tipo.BOOL:
                return CheckCondicao((bool)valor, (bool)GetVariable<bool>().value);
            default:
                return false;
        }
    }

    ISaveVariable<T> GetVariable<T>() {
        if (isGlobal) {
            return new GlobalVariable<T>(nomeVariavel);
        } else {
            return new LocalVariable<T>(nomeVariavel);
        }
    }

    protected bool CheckCondicao(int valor, int variavel) {
        return comparacao switch {
            CondicaoParams.Comparacao.MAIOR => variavel > valor,
            CondicaoParams.Comparacao.MENOR => variavel < valor,
            CondicaoParams.Comparacao.IGUAL => variavel == valor,
            CondicaoParams.Comparacao.DIFERENTE => variavel != valor,
            CondicaoParams.Comparacao.MAIOR_IGUAL => variavel >= valor,
            CondicaoParams.Comparacao.MENOR_IGUAL => variavel <= valor,
            _ => false
        };
    }
    
    protected bool CheckCondicao(float valor, float variavel) {
        return comparacao switch {
            CondicaoParams.Comparacao.MAIOR  => variavel > valor,
            CondicaoParams.Comparacao.MENOR => variavel < valor,
            CondicaoParams.Comparacao.IGUAL => variavel == valor,
            CondicaoParams.Comparacao.DIFERENTE => variavel != valor,
            CondicaoParams.Comparacao.MAIOR_IGUAL => variavel >= valor,
            CondicaoParams.Comparacao.MENOR_IGUAL => variavel <= valor,
            _ => false
        };
    }

    protected bool CheckCondicao(string valor, string variavel) {
        return comparacao switch {
            CondicaoParams.Comparacao.IGUAL => variavel == valor,
            CondicaoParams.Comparacao.DIFERENTE => variavel != valor,
            _ => false
        };
    }

    protected bool CheckCondicao(bool valor, bool variavel) {
        return comparacao switch {
            CondicaoParams.Comparacao.IGUAL => variavel == valor,
            CondicaoParams.Comparacao.DIFERENTE => variavel != valor,
            _ => false
        };
    }
}