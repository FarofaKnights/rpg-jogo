using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class PortaCondicional : MonoBehaviour {

    public Collider col;

    public Collider[] doorCols;

    Interagivel interagivel;

    public CondicaoInfo condicaoInfo;


    public Animator[] animacoes; 

    Condicao condicao;

    public bool consumirItem = false;
    public bool isVault = false;

    void Start() {

        col = GetComponent<SphereCollider>();
        interagivel = GetComponent<Interagivel>();
        condicao = condicaoInfo.GetCondicao();
        condicao.Then(() => {
            if (consumirItem && condicao.GetType() == typeof(CondicaoTemItem)) {
                CondicaoTemItem cond = (CondicaoTemItem)condicao;
                Player.instance.inventario.RemoveItem(cond.itemData, cond.quantidade);
            }
        });

        if (GetComponent<Interagivel>() != null) {
            Interagivel inte = GetComponent<Interagivel>();
            if (inte.tagFilter == null) inte.tagFilter = "Player";
        }

        if(isVault)
        {
            AudioManager.instance.vaultSlam.Play();
        }
    }

    public void OnPlayerInteract() {
        if(condicao.Realizar())
        {
            foreach(Animator anim in animacoes)
            {
                anim.SetTrigger("Abrir");
                AudioManager.instance.doorOpen.Play();
            }
            foreach (Collider col in doorCols)
            {
                col.enabled = false;
            }
            Destroy(this.gameObject);
        }
        else
        {
            AudioManager.instance.doorClose.Play();
        }
    }
}
