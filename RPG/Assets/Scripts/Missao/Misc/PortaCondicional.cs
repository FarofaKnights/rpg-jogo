using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class PortaCondicional : MonoBehaviour {
    public GameObject porta;

    public CondicaoInfo condicaoInfo;

    Condicao condicao;

    void Start() {
        condicao = condicaoInfo.GetCondicao();
        condicao.Then(() => {
            Destroy(porta);
        });
    }

    public void OnPlayerInteract() {
        condicao.Realizar();
    }
}
