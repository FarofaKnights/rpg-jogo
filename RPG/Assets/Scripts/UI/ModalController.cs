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
    bool hideModalOnYes = true;
    bool hideModalOnNo = true;

    public QuestionCallback OpenModal(string txt) {
        texto.text = txt;
        callback = new QuestionCallback();
        modal.SetActive(true);
        
        hideModalOnYes = true;
        hideModalOnNo = true;

        return callback;
    }

    public void CloseModal() {
        modal.SetActive(false);
    }

    public void HandleYes() {
        callback.AnsweredYes();
        if (hideModalOnYes) modal.SetActive(false);
        callback = null;
    }

    public void HandleNo() {
        callback.AnsweredNo();
        if (hideModalOnNo) modal.SetActive(false);
        callback = null;
    }

    public void OnYes(System.Action callbackAction) {
        callback.OnYes(callbackAction);
    }

    public void OnYes(System.Action callbackAction, bool hideModal) {
        hideModalOnYes = hideModal;
        OnYes(callbackAction);
    }

    public void OnNo(System.Action callbackAction) {
        callback.OnNo(callbackAction);
    }

    public void OnNo(System.Action callbackAction, bool hideModal) {
        hideModalOnNo = hideModal;
        OnNo(callbackAction);
    }
}
