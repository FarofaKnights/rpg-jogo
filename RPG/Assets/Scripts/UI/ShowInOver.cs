using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShowInOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public bool comecaLigado = true;
    public float tempoFade = 0.5f;
    public UnityEvent evento;
    public Image[] images;
    bool isOver = false;
    Dictionary<Image, float> targetTransparency = new Dictionary<Image, float>();

    Coroutine corotinaAtual = null;

    void Awake() {
        SetupTransparency();
    }

    void SetupTransparency() {
        targetTransparency.Clear();

        foreach (Image img in images) {
            float transparency = img.color.a;
            targetTransparency[img] = transparency;

            Color cor = img.color;
            cor.a *= comecaLigado ? 1 : 0;

            img.color = cor;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        isOver = true;
        SetTrocarValor(isOver);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        isOver = false;
        SetTrocarValor(isOver);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        evento?.Invoke();
    }


    void SetTrocarValor(bool mostrar = true) {
        if (corotinaAtual != null) StopCoroutine(corotinaAtual);
        corotinaAtual = StartCoroutine(TrocarValor(mostrar));
    }

    IEnumerator TrocarValor(bool mostrar = true) {
        float comeco = mostrar ? 0.0f : 1.0f;
        float fim = mostrar ? 1.0f : 0.0f;
        float stepDir = fim - comeco;
        float a = comeco;

        yield return Lerp(tempoFade, a => {
            if (stepDir < 0) a = 1-a;

            foreach (Image img in images) {
                float t = targetTransparency[img];

                Color cor = img.color;
                cor.a = t * a;

                img.color = cor;
            }
        });

        corotinaAtual = null;
    }


    public IEnumerator Lerp(float duration, System.Action<float> action) {
        float time = 0;
        while (time < duration) {
            float delta = Time.deltaTime;
            float t = (time + delta > duration) ? 1 : (time / duration);
            action(t);
            time += delta;
            yield return null;
        }
        action(1);
    }
}
