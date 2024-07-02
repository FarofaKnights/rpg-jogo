using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsController {
    public int destreza, forca, vida, calor;

    public float forcaPercent;
    public float vidaPercent;
    public float calorPercent;


    public System.Action<string, int> OnChange;

    public StatsController(int destreza, int forca, int vida, int calor) {
        this.destreza = destreza;
        this.forca = forca;
        this.vida = vida;
        this.calor = calor;
    }

    public void TriggerChange(string stat, int value) {
        if (OnChange != null) {
            OnChange(stat, value);
        }
    }

    public void TriggerChange() {
        ForEach(TriggerChange);
    }

    public void AddDestreza(int destreza) {
        this.destreza += destreza;
        TriggerChange("destreza", this.destreza);
    }

    public void AddForca(int forca) {
        this.forca += forca;
        TriggerChange("forca", this.forca);
    }

    public void AddVida(int vida) {
        this.vida += vida;
        TriggerChange("vida", this.vida);
    }

    public void AddCalor(int calor) {
        this.calor += calor;
        TriggerChange("calor", this.calor);
    }

    public void SetDestreza(int destreza) {
        this.destreza = destreza;
        TriggerChange("destreza", this.destreza);
    }

    public void SetForca(int forca) {
        this.forca = forca;
        TriggerChange("forca", this.forca);
    }

    public void SetVida(int vida) {
        this.vida = vida;
        TriggerChange("vida", this.vida);
    }

    public void SetCalor(int calor) {
        this.calor = calor;
        TriggerChange("calor", this.calor);
    }

    public float GetAdicionalVida(float vidaValue) {
        return ((vida-1) * vidaPercent) * vidaValue;
    }

    public float GetAdicionalForca(float danoValue) {
        return ((forca-1) * forcaPercent) * danoValue;
    }

    public float GetAdicionalCalor(float calorValue) {
        return ((calor-1) * calorPercent) * calorValue;
    }

    public void ForEach(System.Action<string, int> action) {
        action("destreza", destreza);
        action("forca", forca);
        action("vida", vida);
        action("calor", calor);
    }
}
