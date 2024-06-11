using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class PortaCondicional : MonoBehaviour {
    public CondicaoParams parametros;
    public GameObject porta;

    Condicao condicao;

    void Start() {
        condicao = new CondicaoIfVariavel(parametros);
        condicao.OnRealizada += () => {
            Destroy(porta);
        };
    }

    public void OnPlayerInteract() {
        condicao.Realizar();
    }
}
