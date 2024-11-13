using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtributoInt: IAtributo<int> {
    public System.Action<int,int> OnChange { get; set; }

    public int valor;
    public int valorMaximo, valorMinimo;
    public int valorMaximoBase;
    public bool hasMaximo = true;

    public AtributoInt(int valor, int valorMaximo, int valorMinimo = 0) {
        this.valor = valor;
        this.valorMaximo = valorMaximo;
        this.valorMinimo = valorMinimo;
        this.valorMaximoBase = valorMaximo;
    }

    public AtributoInt(int valor) {
        this.valor = valor;
        this.hasMaximo = false;
    }

    public int Get() {
        return valor;
    }

    public int GetMax() {
        return valorMaximo;
    }

    public int GetMin() {
        return valorMinimo;
    }

    public int GetMaxBase() {
        return valorMaximoBase;
    }

    public void Set(int valor) {
        this.valor = valor;
        OnChange?.Invoke(valor, valorMaximo);
    }

    public void SetMax(int valor) {
        this.valorMaximo = valor;
        hasMaximo = true;
        OnChange?.Invoke(valor, valorMaximo);
    }

    public void SetMin(int valor) {
        this.valorMinimo = valor;
    }

    public void Add(int valor) {
        if (valor < 0) {
            Sub(-valor);
            return;
        }

        int v = this.valor;
        v += valor;
        if (hasMaximo && v > this.valorMaximo) v = this.valorMaximo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void AddMax(int valor) {
        if (valor < 0) {
            SubMax(-valor);
            return;
        }

        if (!hasMaximo) return;

        int v = this.valorMaximo;
        v += valor;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Sub(int valor) {
        if (valor < 0) {
            Add(-valor);
            return;
        }

        int v = this.valor;
        v -= valor;
        if (v < valorMinimo) v = valorMinimo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void SubMax(int valor) {
        if (valor < 0) {
            AddMax(-valor);
            return;
        }

        if (!hasMaximo) return;

        int v = this.valorMaximo;
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
