using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PossuiVida))]
public class Inimigo : MonoBehaviour {
    [Header("Referência de Componentes")]
    public CharacterController controller;
    public Animator animator;
    public Text vidaText;

    [Header("Configurações de Movimento")]
    public float speed = 3f;
    public Vector3 target;
    Vector3 moveDirection = Vector3.zero;

    public Vector2 timerRange = new Vector2(1, 3);
    float timer;

    [Header("Configurações de Drop")]
    public int recompensaPecas = 10;

    [Header("Configurações de Ataque")]
    public GameObject ataqueHitbox;
    public int dano = 10;
    public float tempoHitbox = 0.5f;
    
    PossuiVida vidaController;

    void Awake() {
        vidaController = GetComponent<PossuiVida>();

        vidaController.onChange += (vida) => {
            float porcentagem = vida / vidaController.VidaMax;
            vidaText.text = (porcentagem * 100).ToString("0") + "%";
        };

        vidaController.onDeath += () => {
            Player.instance.AddPecas(recompensaPecas);
        };

        ataqueHitbox.GetComponent<OnTrigger>().onTriggerEnter += OnHit;
        ataqueHitbox.SetActive(false);
    }


    void Update()  {
        /*
        if (target != Vector3.zero) {
            Move();
        }
        */

        if (timer <= 0) {
            Attack();
            timer = Random.Range(timerRange.x, timerRange.y);
        } else {
            timer -= Time.deltaTime;
        }
    }

    void Move() {
        moveDirection = target - transform.position;
        moveDirection = moveDirection.normalized;
        moveDirection.y = 0;
        moveDirection *= speed;

        controller.Move(moveDirection * Time.deltaTime);

        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.z);
    }

    void Attack() {
        animator.SetTrigger("Attack");
        

        ataqueHitbox.SetActive(false);
        ataqueHitbox.SetActive(true);

        StartCoroutine(DesativarHitbox());
    }

    IEnumerator DesativarHitbox() {
        yield return new WaitForSeconds(tempoHitbox);
        ataqueHitbox.SetActive(false);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ataque")) {
            target = collision.transform.position;
        }
    }

    public void TomarDano(int danoTomado) {
        Debug.Log("Tomou dano: " + danoTomado);
        GetComponent<PossuiVida>().LevarDano(danoTomado);
    }

    public void CurarVida(int cura) {
        GetComponent<PossuiVida>().Curar(cura);
    }

    public void OnHit(GameObject player) {
        player.GetComponent<PossuiVida>().LevarDano(dano);
    }
}
