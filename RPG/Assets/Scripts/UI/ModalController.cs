using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCallback {
    System.Action yesAction, noAction;

    public QuestionCallback() {
        yesAction = null;
        noAction = null;
    }

    public QuestionCallback OnYes(System.Action action) {
        yesAction += action;
        return this;
    }

    public QuestionCallback OnNo(System.Action action) {
        noAction += action;
        return this;
    }

    public QuestionCallback AnsweredYes() {
        yesAction?.Invoke();
        yesAction = null;
        noAction = null;

        return this;
    }

    public QuestionCallback AnsweredNo() {
        noAction?.Invoke();
        noAction = null;
        yesAction = null;
        
        return this;
    }
}

public class ModalController : MonoBehaviour {
    public Text texto;
    public GameObject modal;
    QuestionCallback callback;

    public QuestionCallback OpenModal(string texto) {
        texto.text = texto;
        callback = new QuestionCallback();
        modal.SetActive(true);
        return callback;
    }

    public void CloseModal() {
        modal.SetActive(false);
    }

    public void HandleYes() {
        callback.AnsweredYes();
    }

    public void HandleNo() {
        callback.AnsweredNo();
    }

    public void OnYes(System.Action callback) {
        callback.OnYes(callback);
    }

    public void OnNo(System.Action callback) {
        callback.OnNo(callback);
    }
}
