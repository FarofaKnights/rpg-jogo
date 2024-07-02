using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtributoInt: IAtributo<int> {
    public System.Action<int,int> OnChange { get; set; }

    public int valor;
    public int valorMaximo;
    public int valorMaximoBase;
    public bool hasMaximo = true;

    public AtributoInt(int valor, int valorMaximo) {
        this.valor = valor;
        this.valorMaximo = valorMaximo;
        this.valorMaximoBase = valorMaximo;
    }

    public AtributoInt(string nome, int valor) {
        this.valor = valor;
        this.hasMaximo = false;
    }

    public int Get() {
        return valor;
    }

    public int GetMax() {
        return valorMaximo;
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

    public void Add(int valor) {
        int v = this.valor;
        v += valor;
        if (hasMaximo && v > this.valorMaximo) v = this.valorMaximo;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void AddMax(int valor) {
        if (!hasMaximo) return;

        int v = this.valorMaximo;
        v += valor;
        this.valorMaximo = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void Sub(int valor) {
        int v = this.valor;
        v -= valor;
        if (v < 0) v = 0;
        this.valor = v;
        OnChange?.Invoke(this.valor, this.valorMaximo);
    }

    public void SubMax(int valor) {
        if (!hasMaximo) return;

        int v = this.valorMaximo;
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
