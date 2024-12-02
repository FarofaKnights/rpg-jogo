using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Olá, você tomou a escolha de se aventurar no script de Boss... Boa sorte, você vai precisar.
// O motivo dele ser assim é uma bola de neve de múltiplos fatores que ocuparam o tempo de desenvolvimento.
// E por termos uma entrega semanal definida, que caso não ocorra afeta diretamente na nota,
// não há tempo pra melhorar e aplicar padrões, refatorar e etc. Ele está assim por escassez de tempo.
// Então não procure um motivo racional para ele estar assim, é meramente convenções e calhamentos.
// Talvez no meio de todo o caos, algum sentido possa ser encontrado. Talvez.

[RequireComponent(typeof(PossuiVida))]
public class SniperController : MonoBehaviour {
    public QuestInfo questBoss;
    PossuiVida vidaController;
    public Projetil projetil;
    public Transform pontaSniper;
    public LineRenderer lineRenderer;
    public NavMeshAgent agent;
    public CharacterController controller;
    public Animator animator;
    public LayerMask floor;

    enum EstadoPulo { PULANDO, CAINDO, PARADO }
    EstadoPulo estadoPulo = EstadoPulo.PARADO;

    public enum EstadoFase { PARADO, ATIRANDO, FASE3, FASE4 }
    EstadoFase estadoFase = EstadoFase.PARADO;


    public float dano = 40f;
    public float coronhadaDano = 10f;
    DamageInfo danoTiro, danoCoronhada;
    public GameObject explosaoPrefab;
    
    bool mirando = false;
    // bool movendoController = false;

    Vector3 exitVector;

    public float reloadStunTime = 5f;
    float reloadStunTimer = 0f;
    bool canStun = true;

    [Header("Boss Ataque e tals")]
    public GameObject pistolaPrefab, pistolaPonta;
    public GameObject hitboxPrefab, hitboxHolder;
    GameObject hitboxInstantiated;


    void Awake() {
        danoTiro = new DamageInfo { dano=dano, origem=gameObject, tipoDeDano=TipoDeDano.Projetil, formaDeDano=FormaDeDano.Ativo };
        danoCoronhada = new DamageInfo { dano=coronhadaDano, origem=gameObject, tipoDeDano=TipoDeDano.Melee, formaDeDano=FormaDeDano.Ativo };

        vidaController = GetComponent<PossuiVida>();

        vidaController.SetDestroyOnDeath(false);

        vidaController.onChange += (vida) => {
            if (canStun) {
                canStun = false;
                animator.SetTrigger("Hitted");
                reloadStunTime = reloadStunTimer;
            }

            UIController.HUD.UpdateBossVida(vidaController.Vida, vidaController.VidaMax);
        };

        vidaController.onDeath += (DamageInfo danoInfo) => {
            vidaController.CurarTotalmente();

            if (estadoFase == EstadoFase.FASE3){
                QuestManager.instance.TriggerQuest(questBoss.name, "acabaFase3");
                SetEstadoFase(EstadoFase.FASE4);
            }
                
        };
    }

    public void SetEstadoFase(EstadoFase estado) {
        estadoFase = estado;

        switch (estadoFase) {
            case EstadoFase.PARADO:
                vidaController.SetInvulneravel(true);
                break;
            case EstadoFase.ATIRANDO:
                vidaController.SetInvulneravel(true);
                break;
            case EstadoFase.FASE3:
                vidaController.SetInvulneravel(false);
                UIController.HUD.ShowBossVida(true);
                UIController.HUD.UpdateBossVida(vidaController.Vida, vidaController.VidaMax);
                break;
            case EstadoFase.FASE4:
                vidaController.SetInvulneravel(true);
                UIController.HUD.ShowBossVida(false);
                break;
        }
    }

    public void Mirar() {
        mirando = true;
    }

    public void PararDeMirar() {
        mirando = false;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = false;
    }

    public void Atirar() {
        PararDeMirar();

        Transform target = Player.instance.meio.transform;
        exitVector = target.position - pontaSniper.position;

        RaycastHit hit;
        if (Physics.Raycast(pontaSniper.position, exitVector, out hit)) {
            Acertou(hit.transform);
        }
    }

    void FixedUpdate() {
        RaycastHit hit;

        if (mirando) {
            Transform target = Player.instance.meio.transform;
            Vector3 fromPontaToTarget = target.position - pontaSniper.position;

            if (Physics.Raycast(pontaSniper.position, fromPontaToTarget, out hit)) {
                lineRenderer.SetPosition(0, pontaSniper.position);
                lineRenderer.SetPosition(1, hit.point);
                lineRenderer.enabled = true;
            }
        }

        Vector3 directionMove = transform.forward * 2f;
        

        switch (estadoPulo) {
            case EstadoPulo.PULANDO:
                directionMove += transform.up;
                controller.Move(directionMove * Time.fixedDeltaTime);
                break;
            case EstadoPulo.CAINDO:
                if (Physics.Raycast(controller.transform.position, Vector3.down, out hit, 0.1f, floor)) {
                    estadoPulo = EstadoPulo.PARADO;
                    animator.SetBool("Pulando", false);
                    controller.enabled = false;
                } else {
                    directionMove.y -= 9.8f;
                    controller.Move(directionMove * Time.fixedDeltaTime);
                }
                break;
            case EstadoPulo.PARADO:
                break;
        }

        if (reloadStunTime > 0) {
            reloadStunTime -= Time.fixedDeltaTime;
            if (reloadStunTime <= 0) {
                canStun = true;
            }
        }
    }

    void Acertou(Transform hittedTarget) {
        if (hittedTarget == null) return;

        if (hittedTarget.GetComponent<PossuiVida>() != null) {
            hittedTarget.GetComponent<PossuiVida>().LevarDano(danoTiro);
        }

        if (explosaoPrefab != null)
            Instantiate(explosaoPrefab, hittedTarget.position, Quaternion.identity);
    }

    public void AlertarPlayer() {
        Debug.Log("Player alertado");
    }

    public void SetaPulando() {
        controller.enabled = true;
        agent.enabled = false;

        estadoPulo = EstadoPulo.PULANDO;
    }

    public void SetaCaindo() {
        controller.enabled = true;
        agent.enabled = false;

        estadoPulo = EstadoPulo.CAINDO;
    }
    
    public void SetarFase(int fase) {
        switch (fase) {
            case 1:
                SetEstadoFase(EstadoFase.ATIRANDO);
                break;
            case 2:
                SetEstadoFase(EstadoFase.PARADO);
                break;
            case 3:
                SetEstadoFase(EstadoFase.FASE3);
                break;
            case 4:
                SetEstadoFase(EstadoFase.FASE4);
                break;
        }
    }

    public void PistolaAtirar() {
        GameObject projetil = GameObject.Instantiate(pistolaPrefab, pistolaPonta.transform.position, pistolaPonta.transform.rotation);
        projetil.transform.forward = pistolaPonta.transform.forward;

        Projetil p = projetil.GetComponent<Projetil>();
        if (p != null)
        {
            p.ignoreList.Add(gameObject);
        }
    }

    public void StartCoronhada() {
        // return;
        hitboxInstantiated = GameObject.Instantiate(hitboxPrefab, hitboxHolder.transform);
        hitboxInstantiated.AddComponent<OnTrigger>().onTriggerEnter += OnEnterHitbox;

        int layer = LayerMask.NameToLayer("Ataque");
        hitboxInstantiated.layer = layer;
    }

    public void OnEnterHitbox(GameObject hit) {
        if (hit.GetComponent<PossuiVida>() != null) {
            hit.GetComponent<PossuiVida>().LevarDano(danoCoronhada);
            if (hit.CompareTag("Player"))
                GameManager.instance.StartCoroutine(SlowdownOnHit());
        }
    }
    IEnumerator SlowdownOnHit() {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.05f);
        if (!GameManager.instance.IsPaused()) Time.timeScale = 1;
        else Time.timeScale = 0;
    }


    public void EndCoronhada() {
        Destroy(hitboxInstantiated);
    }
}
