using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interagivel))]
public class PortaCondicional : MonoBehaviour {
    public float rotationSpeed;

    public Collider col;

    Interagivel interagivel;

    public CondicaoInfo condicaoInfo;

    public AudioSource abriu;
    public AudioSource naoAbriu;

    public Animator[] animacoes; 

    Condicao condicao;

    public bool consumirItem = false;

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
    }

    public void OnPlayerInteract() {
        if(condicao.Realizar())
        {
            foreach(Animator anim in animacoes)
            {
                anim.SetTrigger("Abrir");
                AudioManager.instance.doorOpen.Play();
            }
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("nao");
            AudioManager.instance.doorClose.Play();
        }
    }
}
