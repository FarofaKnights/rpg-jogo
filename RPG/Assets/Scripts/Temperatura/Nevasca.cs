using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FogConfig {
    public float fogDensity;
    public Color fogColor;
}

public class Nevasca : MonoBehaviour {
    public float temperatura;
    public float changeSpeed = 1f;
    public FogConfig normal, nevasca;
    FogConfig currentConfig, targetConfig;

    public Collider[] triggers;
    List<OnTrigger> playerOnTrigger = new List<OnTrigger>();

    bool isInNevasca = false;
    bool changing = true;

    void Start() {
        currentConfig = new FogConfig();
        currentConfig.fogDensity = normal.fogDensity;
        currentConfig.fogColor = normal.fogColor;

        foreach (Collider trigger in triggers) {
            OnTrigger triggerScript = trigger.GetComponent<OnTrigger>();
            if (triggerScript == null) triggerScript = trigger.gameObject.AddComponent<OnTrigger>();
            triggerScript.tagFilter = "Player";
            
            triggerScript.onTriggerEnter += (GameObject player) => OnEntered(triggerScript, player);
            triggerScript.onTriggerExit += (GameObject player) => OnExited(triggerScript, player);
        }

        TemperaturaController.instance.DefinirArea(GetInstanceID(), temperatura);
    }

    void OnEntered(OnTrigger trigger, GameObject player) {
        if (playerOnTrigger.Contains(trigger)) return;
        playerOnTrigger.Add(trigger);
        if (playerOnTrigger.Count == 1) {
            isInNevasca = true;
            changing = true;
            TemperaturaController.instance.AdicionarSentidor(player.GetComponent<Sentidor>(), GetInstanceID());
        }
    }

    void OnExited(OnTrigger trigger, GameObject player) {
        playerOnTrigger.Remove(trigger);
        if (playerOnTrigger.Count == 0) {
            isInNevasca = false;
            changing = true;
            TemperaturaController.instance.RemoverSentidor(player.GetComponent<Sentidor>(), GetInstanceID());
        }
    }

    public void SetConfig(FogConfig config) {
        RenderSettings.fogDensity = config.fogDensity;
        RenderSettings.fogColor = config.fogColor;
    }

    void FixedUpdate() {
        if (!changing) return;

        targetConfig = isInNevasca ? nevasca : normal;
        currentConfig.fogDensity = Mathf.Lerp(currentConfig.fogDensity, targetConfig.fogDensity, changeSpeed * Time.fixedDeltaTime);
        currentConfig.fogColor = Color.Lerp(currentConfig.fogColor, targetConfig.fogColor, changeSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(currentConfig.fogDensity-targetConfig.fogDensity) < 0.01f) {
            changing = false;
            currentConfig.fogDensity = targetConfig.fogDensity;
            currentConfig.fogColor = targetConfig.fogColor;
        }

        SetConfig(currentConfig);
    }

}
