using System.Collections;
using System.Collections.Generic;
using System;

public interface IAtributo<T> {
    Action<T,T> OnChange { get; set; }

    void Set(T valor);
    void SetMax(T valor);
    void Add(T valor);
    void AddMax(T valor);
    void Sub(T valor);
    void SubMax(T valor);
    void Reset();
    T Get();
    T GetMax();
}