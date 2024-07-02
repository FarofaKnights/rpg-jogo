using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtributosController {
    public IAtributo<float> vida;
    public IAtributo<float> calor;
    public IAtributo<int> pecas;
    bool inicializado = false;

    public void Initialize() {
        if (inicializado) return;
        inicializado = true;

        if (calor == null) calor = new AtributoFloat(100, 100);
        if (pecas == null) pecas = new AtributoInt(100, 100);
        if (vida == null) vida = Player.instance.GetComponent<PossuiVida>().GetVidaAtributo();

        calor.OnChange += UIController.HUD.UpdateCalor;
        pecas.OnChange += (valor, max) => UIController.HUD.UpdatePecas(valor);
        vida.OnChange += UIController.HUD.UpdateVida;
    }
}
