using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class Cutscene: MonoBehaviour {
    public PlayableDirector timeline;
    public bool playOnStart = false;
    public bool destruirAoFim = false;
    public GameObject[] ativarNaCutscene;
    public GameObject[] desativarNaCutscene;

    public System.Action onEnd;

    bool chamouFadeInSemFadeOut = false;
    

    void Start() {
        if (playOnStart) {
            Comecar();
        }
    }


    void OnEnable() {
        timeline.stopped += OnFim;
    }

    void OnDisabled() {
        timeline.stopped -= OnFim;
    }

    public void FadeIn() {
        UIController.instance.FadeIn();
        chamouFadeInSemFadeOut = true;
    }

    public void FadeOut() {
        UIController.instance.FadeOut();
        chamouFadeInSemFadeOut = false;
    }

    public void ShowLegenda(string caption) {
        UIController.dialogo.ShowFalaTexto(caption);
    }

    public void Comecar() {
        StartCoroutine(ComecarCoroutine());
    }

    public IEnumerator ComecarCoroutine() {
        yield return UIController.instance.FadeInAsync();
        
        UIController.dialogo.ShowFalaTexto("");
        UIController.dialogo.gameObject.SetActive(false);
        GameManager.instance.SetCutsceneMode(true);
        GameManager.instance.SetInimigosAtivos(false);

        foreach (GameObject obj in ativarNaCutscene) {
            if (obj == null) continue;
            obj.SetActive(true);
        }
        foreach (GameObject obj in desativarNaCutscene) {
            if (obj == null) continue;
            obj.SetActive(false);
        }

        timeline.Play();
    }

    public void OnFim(PlayableDirector director) {
        if (director.time >= director.duration) {
            OnFim();
        }
    }

    public void OnFim() {
        StartCoroutine(OnFimCoroutine());
    }

    public IEnumerator OnFimCoroutine() {
        Debug.Log("Fim da cutscene");
/*
        if (!chamouFadeInSemFadeOut) {
           
        }*/

        yield return UIController.instance.FadeOutAsync();

        foreach (GameObject obj in ativarNaCutscene) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in desativarNaCutscene) {
            obj.SetActive(true);
        }

        UIController.dialogo.ShowFalaTexto("");
        UIController.dialogo.gameObject.SetActive(true);
        GameManager.instance.SetCutsceneMode(false);
        GameManager.instance.SetInimigosAtivos(true);

        if (destruirAoFim) {
            Destroy(gameObject);
        }

        onEnd?.Invoke();
        onEnd = null;
    }
}
