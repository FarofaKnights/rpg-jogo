using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletarSe : MonoBehaviour {
    public CondicaoInfo condicaoInfo;
    Condicao condicao;

    void Start()  {
        condicao = condicaoInfo.GetCondicao();
        condicao.Then(() => {
            Destroy(gameObject);
        });

    }
}
