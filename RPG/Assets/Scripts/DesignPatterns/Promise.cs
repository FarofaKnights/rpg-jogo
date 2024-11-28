using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PromiseState {
    Pending,
    Resolved,
    Rejected
}

public class Promise {
    System.Action onThen, onCatch;
    PromiseState _state = PromiseState.Pending;

    public PromiseState state {
        get {
            return _state;
        }
    }

    public Promise Then(System.Action action) {
        if (_state == PromiseState.Resolved) {
            action();
            return this;
        } else if (_state == PromiseState.Rejected) {
            return this;
        }

        onThen += action;
        return this;
    }

    public Promise Catch(System.Action action) {
        if (_state == PromiseState.Rejected) {
            action();
            return this;
        } else if (_state == PromiseState.Resolved) {
            return this;
        }

        onCatch += action;
        return this;
    }

    public void Resolve() {
        if (_state == PromiseState.Pending) {
            _state = PromiseState.Resolved;
            onThen?.Invoke();
            onThen = null;
            onCatch = null;
        }
    }

    public void Reject() {
        if (_state == PromiseState.Pending) {
            _state = PromiseState.Rejected;
            onCatch?.Invoke();
            onCatch = null;
            onThen = null;
        }
    }

}
