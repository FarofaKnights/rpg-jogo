using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletarSe : MonoBehaviour {
    public CondicaoInfo condicaoInfo;
    Condicao condicao;

    IEnumerator Start()  {
        yield return null;

        condicao = condicaoInfo.GetCondicao();
        condicao.Then(() => {
            Destroy(gameObject);
        });
    }
}
