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

    public void OnToggle(bool isOn) {
        // Caso você clique no toggle para desativa-lo, é possivel que o parametro venha como true mesmo que internamente ele esteja false
        // Por isso, é melhor pegar o valor direto do toggle (aprendendo na prática, isso resolve)
        isOn = toggle.isOn; 

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
