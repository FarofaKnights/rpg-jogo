using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class FluxoDialogo {
    public Fala[] falas;
    public Fala currentFala;
    Escolhas currentEscolhas;
    Escolha currentEscolha;
    int index = 0;
    public System.Action OnEnd;

    FluxoDialogo subFluxo = null;
    IEnumerator autoNextCoroutine;

    public FluxoDialogo(Fala[] falas, System.Action OnEnd = null) {
        this.falas = falas;
        this.OnEnd = OnEnd;
    }

    public FluxoDialogo(Dialogo dialogo, System.Action OnEnd = null) {
        this.falas = dialogo.falas.ToArray();
        this.OnEnd = OnEnd;
    }

    public void Proximo() {
        if (subFluxo != null) {
            subFluxo.Proximo();
            return;
        }

        ShowFala();
    }

    public void ShowFala() {
        if (falas == null) return;
        if (index >= falas.Length) {
            HandleEnd();
            return;
        }

        if (autoNextCoroutine != null) {
            GameManager.instance.StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }

        Fala fala = falas[index];
        index++;
        ProcessarFala(fala);
    }

    void ProcessarFala(Fala fala) {
        currentFala = fala;

        if (fala.escolhas == null || fala.escolhas.tipoEscolhas == Escolhas.TipoEscolhas.NULL || fala.escolhas.escolhas.Length == 0) {
            UIController.dialogo.ShowFalaTexto(fala.text);
            ProcessarPosFala(fala);
        }
        else {
            currentEscolhas = fala.escolhas;
            UIController.dialogo.ShowEscolhas(fala.escolhas, HandleEscolha);
        }
    }

    void HandleEscolha(Escolha escolha) {
        if (escolha == null) {
            TrueSubfluxoEnd();
            return;
        }

        currentEscolha = escolha;
        Fala[] falas;

        if (currentEscolha.tipoEscolha == Escolha.TipoEscolha.FALA) {
            Fala fala = new Fala(currentEscolha.respostaFala);
            falas = new Fala[] { fala };
        }
        else if (currentEscolha.tipoEscolha == Escolha.TipoEscolha.DIALOGO) falas = currentEscolha.respostaDialogo.falas.ToArray();
        else {
            HandleSubfluxoEnd();
            return;
        }
    
        subFluxo = new FluxoDialogo(falas, HandleSubfluxoEnd);
        subFluxo.Proximo();
    }

    void HandleSubfluxoEnd() {
        subFluxo = null;

        int escolhaIndex = -1;
        for (int i = 0; i < currentEscolhas.escolhas.Length; i++) {
            if (currentEscolhas.escolhas[i] == currentEscolha) {
                escolhaIndex = i;
                break;
            }
        }
        
        if (currentEscolhas.tipoEscolhas != Escolhas.TipoEscolhas.MENU || currentEscolhas.idEscolhaDeSaidaDoMenu == escolhaIndex) {
            TrueSubfluxoEnd();
        } else {
            // Menu repete até escolher uma opção de saída
            index--;
            Proximo();
        }
    }

    void TrueSubfluxoEnd() {
        subFluxo = null;
        ProcessarPosFala(currentFala);
        Proximo();
    }

    void ProcessarPosFala(Fala fala) {
        if (fala.acao != null) {
            Acao acao = fala.acao.GetAcao();
            if (acao != null) {
                acao.Realizar();
            }
        }

        if (fala.autoNext) {
            if (fala.text == "") ShowFala();
            else {
                autoNextCoroutine = AutoNext(fala);
                GameManager.instance.StartCoroutine(autoNextCoroutine);
            }
        }
    }

    IEnumerator AutoNext(Fala waitingFala) {
        yield return new WaitForSeconds(UIController.dialogo.autoNextDelay);
        if (currentFala == waitingFala) ShowFala();
    }

    public void HandleEnd() {
        System.Action end = OnEnd;
        OnEnd = null;
        end?.Invoke();
    }
}

public class DialogoController : MonoBehaviour {
    public Text texto;
    public System.Action OnDialogoEnd;
    public float autoNextDelay = 5f;

    public GameObject dialogoHolder, escolhasHolder;
    public GameObject escolhaPrefab;

    Escolhas escolhasAtual;
    System.Action<Escolha> CallbackEscolhida;
    int currentEscolhaIndex = 0;
    FluxoDialogo fluxoAtual;

    // Para cameras de cutscene
    CinemachineVirtualCamera currentCam;
    bool playerLocked = false;


    void Start() {
        texto.text = "";
    }

    public void ShowFalaTexto(string text) {
        escolhasHolder.SetActive(false);
        dialogoHolder.SetActive(true);

        Debug.Log("Mostrando texto: " + text);
        texto.text = text;
    }

    public void ShowEscolhas(Escolhas escolhas, System.Action<Escolha> CallbackEscolhida) {
        dialogoHolder.SetActive(false);
        escolhasHolder.SetActive(true);

        foreach (Transform child in escolhasHolder.transform) {
            Destroy(child.gameObject);
        }

        currentEscolhaIndex = 0;

        foreach (Escolha escolha in escolhas.escolhas) {
            GameObject escolhaGO = Instantiate(escolhaPrefab, escolhasHolder.transform);
            escolhaGO.GetComponent<Text>().text = escolha.escolha;
        }

        StartCoroutine(RefreshEscolhas());

        escolhasAtual = escolhas;

        GameManager.instance.controls.Dialog.Interact.performed -= Proximo;
        GameManager.instance.controls.Dialog.Up.performed += EscolhaSelectUp;
        GameManager.instance.controls.Dialog.Down.performed += EscolhaSelectDown;
        GameManager.instance.controls.Dialog.Interact.performed += EscolhaSelect;

        this.CallbackEscolhida = CallbackEscolhida;

        UpdateEscolhaSelect();
    }

    IEnumerator RefreshEscolhas() {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(escolhasHolder.GetComponent<RectTransform>());
        UpdateEscolhaSelect();
    }

    public void UpdateEscolhaSelect() {
        for (int i = 0; i < escolhasHolder.transform.childCount; i++) {
            escolhasHolder.transform.GetChild(i).GetComponent<Text>().fontStyle = FontStyle.Normal;
        }

        escolhasHolder.transform.GetChild(currentEscolhaIndex).GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    public void EscolhaSelectUp(InputAction.CallbackContext ctx) {
        currentEscolhaIndex--;
        if (currentEscolhaIndex < 0) currentEscolhaIndex = escolhasHolder.transform.childCount - 1;
        UpdateEscolhaSelect();
    }

    public void EscolhaSelectDown(InputAction.CallbackContext ctx) {
        currentEscolhaIndex++;
        if (currentEscolhaIndex >= escolhasHolder.transform.childCount) currentEscolhaIndex = 0;
        UpdateEscolhaSelect();
    }

    public void EscolhaSelect(InputAction.CallbackContext ctx) {
        escolhasHolder.SetActive(false);
        dialogoHolder.SetActive(true);

        Escolha escolha = escolhasAtual.escolhas[currentEscolhaIndex];

        CallbackEscolhida?.Invoke(escolha);
        CallbackEscolhida = null;

        GameManager.instance.controls.Dialog.Up.performed -= EscolhaSelectUp;
        GameManager.instance.controls.Dialog.Down.performed -= EscolhaSelectDown;
        GameManager.instance.controls.Dialog.Interact.performed -= EscolhaSelect;

        StartCoroutine(ReativarInteract());
    }

    IEnumerator ReativarInteract() {
        yield return new WaitForEndOfFrame();
        GameManager.instance.controls.Dialog.Interact.performed += Proximo;
    }

    public void StartDialogo(Dialogo dialogo, System.Action OnDialogoEnd = null, bool lockPlayer = true) {
        StartDialogo(dialogo.falas.ToArray(), OnDialogoEnd, lockPlayer);
    }

    public void StartDialogo(Fala[] falas, System.Action OnDialogoEnd = null, bool lockPlayer = true) {
        if (fluxoAtual != null) {
            HandleDialogoEnd();
        }

        fluxoAtual = new FluxoDialogo(falas, OnDialogoEnd);

        playerLocked = lockPlayer;
        if (lockPlayer) {
            GameManager.instance.controls.Dialog.Interact.performed += Proximo;
            GameManager.instance.SetIntermediaryState(GameState.Dialog);
        } else {
            GameManager.instance.controls.UI.Interact.performed += Proximo;
        }

        this.OnDialogoEnd += OnDialogoEnd;
        fluxoAtual.OnEnd += HandleDialogoEnd;
        Proximo();
    }


    void Proximo() {
        if (fluxoAtual == null) return;
        fluxoAtual.Proximo();
    }

    void Proximo(InputAction.CallbackContext ctx) {
        Proximo();
    }

    void HandleDialogoEnd() {
        texto.text = "";

        if (playerLocked) {
            GameManager.instance.controls.Dialog.Interact.performed -= Proximo;
            GameManager.instance.RestoreIntermediaryState();
        } else {
            GameManager.instance.controls.UI.Interact.performed -= Proximo;
        }

        fluxoAtual = null;

        System.Action end = OnDialogoEnd;
        OnDialogoEnd = null;
        end?.Invoke();
    }

    public void RemoveDialogoEndEvent() {
        OnDialogoEnd = null;
    }

    public void SetCamera(CinemachineVirtualCamera cam) {
        if (cam == null) {
            Player.instance.LookForward();
        }
        
        if (currentCam != null) {
            currentCam.Priority = 0;
        }

        currentCam = cam;
        if (currentCam != null)
            currentCam.Priority = 20;
    }
}
