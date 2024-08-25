using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfiguracoesUI : MonoBehaviour, UITab {
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);

        if (SettingsManager.instance.hasChanged) {
            SettingsManager.instance.hasChanged = false;
            SettingsManager.instance.WriteValues();
        }
    }
}
