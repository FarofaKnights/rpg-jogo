using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour {
    public GameObject conteudo;
    public Image blackScreen;
    public Text percentageText;

    public float transicaoTempo = 0.5f;

    void Start() {
        GameManager.instance.loading.onProgress += UpdateProgress;
    }

    void UpdateProgress(float progress) {
        percentageText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    public void StartLoading() {
        StartCoroutine(StartLoadingAsync());
    }

    public IEnumerator StartLoadingAsync() {
        conteudo.SetActive(false);
        gameObject.SetActive(true);
        yield return AnimacaoTelaPreta(true, transicaoTempo);
        conteudo.SetActive(true);
        UpdateProgress(0.0f);
        yield return AnimacaoTelaPreta(false, transicaoTempo);
    }

    IEnumerator AnimacaoTelaPreta(bool fadeIn, float duration) {
        float alpha = fadeIn ? 0 : 1;
        float step = fadeIn ? 1 : -1;
        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;
        blackScreen.color = new Color(0, 0, 0, alpha);

        while (Time.unscaledTime < endTime) {
            alpha += step * Time.unscaledDeltaTime / duration;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackScreen.color = new Color(0, 0, 0, fadeIn ? 1 : 0);
    }

    public void LoadingEnded() {
        StartCoroutine(LoadingEndedAsync());
    }

    public IEnumerator LoadingEndedAsync() {
        yield return AnimacaoTelaPreta(true, transicaoTempo);
        conteudo.SetActive(false);
        yield return AnimacaoTelaPreta(false, transicaoTempo);
        gameObject.SetActive(false);
    }
}
