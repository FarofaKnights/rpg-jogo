using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour {
    public Image blackScreen;

    public void StartLoading() {
        gameObject.SetActive(true);
        StartCoroutine(AnimacaoTelaPreta(false, 0.5f));
    }

    IEnumerator AnimacaoTelaPreta(bool fadeIn, float duration) {
        float alpha = fadeIn ? 0 : 1;
        float step = fadeIn ? 1 : -1;
        float startTime = Time.time;
        float endTime = startTime + duration;
        blackScreen.color = new Color(0, 0, 0, alpha);

        while (Time.time < endTime) {
            alpha += step * Time.unscaledDeltaTime / duration;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackScreen.color = new Color(0, 0, 0, fadeIn ? 1 : 0);
    }

    public void LoadingEnded() {
        gameObject.SetActive(false);
        StartCoroutine(AnimacaoTelaPreta(true, 0.5f));
    }
}
