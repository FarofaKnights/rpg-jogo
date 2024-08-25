using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonUI : MonoBehaviour {
    public Text text;
    public Image background;
    public Outline outline;
    
    public Color onOutlineColor, offOutlineColor;
    public Color onBackgroundColor, offBackgroundColor;
    public Color onTextColor, offTextColor;

    public Toggle toggle;

    void Start() {
        toggle.onValueChanged.AddListener(OnToggle);
        OnToggle(toggle.isOn);
    }

    void OnToggle(bool isOn) {
        if (isOn) {
            text.color = onTextColor;
            background.color = onBackgroundColor;
            outline.effectColor = onOutlineColor;
        } else {
            text.color = offTextColor;
            background.color = offBackgroundColor;
            outline.effectColor = offOutlineColor;
        }
    }

    void OnValidate() {
        toggle = GetComponent<Toggle>();
        text = GetComponentInChildren<Text>();
        background = GetComponent<Image>();
        outline = GetComponent<Outline>();

        OnToggle(toggle.isOn);
    }
}
