using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VidaAtributo: IAtributo<float> {
    public System.Action<float, float> OnChange { get; set; }
    PossuiVida vida;
    float vidaMaxBase;

    public VidaAtributo(PossuiVida vida) {
        this.vida = vida;
        vidaMaxBase = vida.VidaMax;
        
        vida.onChange += (valor) => {
            OnChange(valor, vida.VidaMax);
        };
    }

    public float Get() {
        return vida.Vida;
    }

    public float GetMax() {
        return vida.VidaMax;
    }

    public void Set(float valor) {
        vida.SobreescreverVida(valor);
    }

    public void SetMax(float valor) {
        vida.SetarVidaMax(valor);
    }

    public void Add(float valor) {
        vida.Curar(valor);
    }

    public void AddMax(float valor) {
        float max = vida.VidaMax + valor;
        vida.SetarVidaMax(max);
    }

    public void Sub(float valor) {
        vida.LevarDano(valor);
    }

    public void SubMax(float valor) {
        float max = vida.VidaMax - valor;
        vida.SetarVidaMax(max);
    }

    
    public void Reset() {
        SetMax(vidaMaxBase);
        Set(vidaMaxBase);
        OnChange(vidaMaxBase, vidaMaxBase);
    }
}

public class PossuiVida : MonoBehaviour {
    [SerializeField] protected float vida;
    [SerializeField] protected float vidaMax;
    public float Vida { get { return vida; } }
    public float VidaMax { get { return vidaMax; } }

    public System.Action<float> onChange, onHeal, onDamage;
    public System.Action onDeath;
    public Func<float, float> modificadorDeDano;

    public void LevarDano(float dano) {
        float oldVida = vida;

        if (modificadorDeDano != null) {
            dano = modificadorDeDano(dano);
        }

        if (dano <= 0) return;

        vida -= dano;

        if (vida < 0) {
            vida = 0;
            dano = oldVida;
        }
        
        onDamage?.Invoke(dano);
        onChange?.Invoke(vida);
        
        if (vida <= 0) {
            Morrer();
        }
    }

    public void Morrer() {
        onDeath?.Invoke();
        Destroy(gameObject);
    }

    public void Curar(float cura) {
        vida += cura;

        if (vida > vidaMax) {
            vida = vidaMax;
            cura = vidaMax - vida;
        }

        onHeal?.Invoke(cura);
        onChange?.Invoke(vida);
    }

    public void CurarTotalmente() {
        vida = vidaMax;
    }

    public void SobreescreverVida(float vida) {
        this.vida = vida;
        onChange?.Invoke(vida);
    }

    public void SetarVidaMax(float vidaMax) {
        this.vidaMax = vidaMax;
        onChange?.Invoke(vida);
    }

    public IAtributo<float> GetVidaAtributo() {
        return new VidaAtributo(this);
    }
}