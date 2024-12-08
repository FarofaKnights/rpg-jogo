using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtributoItem: IAtributo<int> {
    public System.Action<int,int> OnChange { get; set; }

    public int Get() {
        return Player.Inventario.GetQuantidade(GameManager.instance.pecasItem);
    }

    public int GetMax() {
        return int.MaxValue;
    }

    public int GetMin() {
        return 0;
    }

    public int GetMaxBase() {
        return int.MaxValue;
    }

    public void Set(int valor) {
        Player.Inventario.SetItem(GameManager.instance.pecasItem, valor);
        OnChange?.Invoke(valor, int.MaxValue);
    }

    public void SetMax(int valor) { }

    public void SetMin(int valor) { }

    public void Add(int valor) {
        if (valor < 0) {
            Sub(-valor);
            return;
        }

        Player.Inventario.AddItem(GameManager.instance.pecasItem, valor);
        OnChange?.Invoke(Get(), int.MaxValue);
    }

    public void AddMax(int valor) { }

    public void Sub(int valor) {
        if (valor < 0) {
            Add(-valor);
            return;
        }

        Player.Inventario.RemoveItem(GameManager.instance.pecasItem, valor);
        OnChange?.Invoke(Get(), int.MaxValue);
    }

    public void SubMax(int valor) { }

    public void Reset() {
        Player.Inventario.SetItem(GameManager.instance.pecasItem, 0);
        OnChange?.Invoke(0, int.MaxValue);
    }
}
