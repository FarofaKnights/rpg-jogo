using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicadorMiraUI : MonoBehaviour {
    Image image;

    public Color color;
    public Vector3 targetScale;
    public Vector2 restingPosition;
    public Vector2 aimingPosition;
    public float changeSpeed = 0.1f;
    bool withAim = false;

    void Awake() {
        image = GetComponent<Image>();
    }

    void FixedUpdate() {
        Vector2 targetPosition = withAim ? aimingPosition : restingPosition;
        image.rectTransform.anchoredPosition = Vector2.Lerp(image.rectTransform.anchoredPosition, targetPosition, changeSpeed * Time.fixedDeltaTime);
        image.color = Color.Lerp(image.color, color, changeSpeed * Time.fixedDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, changeSpeed * Time.fixedDeltaTime);
    }

    public void SetAim(bool value) {
        withAim = value;
        targetScale = Vector3.one * (value ? 2f : 1f);
    }
}
