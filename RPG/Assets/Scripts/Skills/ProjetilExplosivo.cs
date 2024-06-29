using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilExplosivo : MonoBehaviour
{
    public float velocidade = 10;
    public float tempoDeVida = 2;
    public float dano = 1;
    public GameObject Explosao;

    public string tagAlvo = "Player";

    bool alreadyHit = false;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        transform.position += transform.forward * velocidade * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagAlvo))
        {
            if (alreadyHit) return;
            alreadyHit = true;

            PossuiVida vida = other.GetComponent<PossuiVida>();
            if (vida != null)
            {
                vida.LevarDano(dano);
            }
            Destroy(gameObject);
        }
        else
        {
            Instantiate(Explosao, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
