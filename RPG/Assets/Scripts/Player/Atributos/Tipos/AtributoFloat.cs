using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtributoFloat: IAtributo<float> {
    public System.Action<float,float> OnChange { get; set; }

    public float valor;
    public float valorMaximo, valorMinimo;
    public float valorMaximoBase;
    public bool hasMaximo = true;

    public AtributoFloat(float valor, float valorMaximo, float valorMinimo = 0) {
        this.valor = valor;
        this.valorMaximo = valorMaximo;
        this.valorMinimo = valorMinimo;
        this.valorMaximoBase = valorMaximo;
    }

    public AtributoFloat(float valor) {
        this.valor = valor;
        this.hasMaximo = false;
    }

    public float Get() {
        return valor;
    }

    public float GetMax() {
        return valorMaximo;
    }

    public float GetMin() {
        return valorMinimo;
    }

    public float GetMaxBase() {
        return valorMaximoBase;
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

    public void SetMin(float valor) {
        this.valorMinimo = valor;
    }

    public void Add(float valor) {
        if (valor < 0) {
            Sub(-valor);
            return;
        }

        float v = this.valor;
        v += valor;
        if (hasMaximo && v > this.valorMaximo) v = this.valorMaximo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void AddMax(float valor) {
        if (valor < 0) {
            SubMax(-valor);
            return;
        }

        if (!hasMaximo) return;

        float v = this.valorMaximo;
        v += valor;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Sub(float valor) {
        if (valor < 0) {
            Add(-valor);
            return;
        }

        float v = this.valor;
        v -= valor;
        if (v < valorMinimo) v = valorMinimo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void SubMax(float valor) {
        if (valor < 0) {
            AddMax(-valor);
            return;
        }

        if (!hasMaximo) return;

        float v = this.valorMaximo;
        v -= valor;
        if (v < valorMinimo) v = valorMinimo;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Reset() {
        this.valor = 0;
        this.valorMaximo = hasMaximo? this.valorMaximoBase : 0;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }
}
