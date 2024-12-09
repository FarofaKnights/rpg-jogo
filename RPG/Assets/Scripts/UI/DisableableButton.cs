using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableableButton : MonoBehaviour {
    public Text text;
    public Image background;
    public Outline outline;
    
    public Color onOutlineColor, offOutlineColor;
    public Color onBackgroundColor, offBackgroundColor;
    public Color onTextColor, offTextColor;

    public Button button;
    bool settedOnce = false;

    void Start() {
        if (!settedOnce)
            SetDisabled(button.interactable);
    }

    public void SetDisabled(bool disabled) {
        button.interactable = !disabled;

        if (!disabled) {
            text.color = onTextColor;
            background.color = onBackgroundColor;
            outline.effectColor = onOutlineColor;
        } else {
            text.color = offTextColor;
            background.color = offBackgroundColor;
            outline.effectColor = offOutlineColor;
        }

        settedOnce = true;
    }

    void OnValidate() {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        background = GetComponent<Image>();
        outline = GetComponent<Outline>();

        SetDisabled(button.interactable);
    }
}
