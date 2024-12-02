using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InimigoComArma : Inimigo, IEquipador {
    [Header("Arma e configurações")]
    public Arma arma; // Sobreescreve o braço caso esteja definido
    public Braco braco;
    public float bracoCooldown = 1f;
    public GameObject armaHolder;
    GameObject equipInstance;

    protected override void Start() {
        base.Start();

        if (arma != null) {
            equipInstance = Instantiate(arma.gameObject);
            arma = equipInstance.GetComponent<Arma>();
            arma.Equip(this);
        } else if (braco != null) {
            equipInstance = Instantiate(braco.gameObject);
            braco = equipInstance.GetComponent<Braco>();
            braco.Equip(this);
        }
    }

    public void Equipar(Equipamento equipamento) {
        Transform equipTransform = equipamento.transform;

        if (equipamento.GetType() == typeof(Arma)) {
            arma = equipamento as Arma;
        } else if (typeof(Braco).IsAssignableFrom(equipamento.GetType())) {
            braco = equipamento as Braco;
        }

        
        equipTransform.SetParent(armaHolder.transform);
        equipTransform.localPosition = Vector3.zero;
        equipTransform.localRotation = Quaternion.identity;
    }

    public void Desequipar(Equipamento equipamento) {
        arma = null;
        Destroy(equipInstance);
    }

    public override AtaqueInstance GetAtaque(out float waitBeforeLeaving) {
        waitBeforeLeaving = 0;
        
        if (arma != null) return arma.Atacar();
        else if (braco != null) {
            braco.Ativar();
            waitBeforeLeaving = bracoCooldown;
            return null;
        }

        return base.GetAtaque(out waitBeforeLeaving);
    }

    public override DamageInfo GetDano() {
        if (arma != null) return arma.GetDano();
        else if (braco != null) return braco.GetDano();

        return base.GetDano();
    }
}
