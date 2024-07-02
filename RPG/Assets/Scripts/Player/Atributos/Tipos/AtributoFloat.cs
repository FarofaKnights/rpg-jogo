using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtributoFloat: IAtributo<float> {
    public System.Action<float,float> OnChange { get; set; }

    public float valor;
    public float valorMaximo;
    public float valorMaximoBase;
    public bool hasMaximo = true;

    public AtributoFloat(float valor, float valorMaximo) {
        this.valor = valor;
        this.valorMaximo = valorMaximo;
        this.valorMaximoBase = valorMaximo;
    }

    public AtributoFloat(string nome, float valor) {
        this.valor = valor;
        this.hasMaximo = false;
    }

    public float Get() {
        return valor;
    }

    public float GetMax() {
        return valorMaximo;
    }

    public void Set(float valor) {
        this.valor = valor;
        OnChange?.Invoke(valor, valorMaximo);
    }

    public void SetMax(float valor) {
        this.valorMaximo = valor;
        hasMaximo = true;
        OnChange?.Invoke(valor, valorMaximo);
    }

    public void Add(float valor) {
        float v = this.valor;
        v += valor;
        if (hasMaximo && v > this.valorMaximo) v = this.valorMaximo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void AddMax(float valor) {
        if (!hasMaximo) return;

        float v = this.valorMaximo;
        v += valor;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Sub(float valor) {
        float v = this.valor;
        v -= valor;
        if (v < 0) v = 0;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void SubMax(float valor) {
        if (!hasMaximo) return;

        float v = this.valorMaximo;
        v -= valor;
        if (v < 0) v = 0;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Reset() {
        this.valor = 0;
        this.valorMaximo = hasMaximo? this.valorMaximoBase : 0;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }
}
