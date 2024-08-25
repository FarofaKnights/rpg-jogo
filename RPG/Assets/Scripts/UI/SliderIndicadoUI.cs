using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderIndicadoUI : MonoBehaviour {
    public Slider slider;
    public Text indicator;

    public void UpdateValue() {
        float min = slider.minValue;
        float max = slider.maxValue;
        float value = slider.value;

        string percentage = ((value - min) / (max - min) * 100).ToString("0") + "%";
        indicator.text = percentage;
    }
}
