using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defective.JSON;

public class LojaController : MonoBehaviour, Saveable {
    public static LojaController instance;

    [Header("Vitrine")]
    public VitrineSlot[] slots;

    [Header("Entrar")]
    public Camera lojaCam;
    Camera oldCam;
    public string animatorTriggerName;
    public Animator animator;
    bool dentro = false;
    
    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.controls.Loja.CheatMode.performed += ctx => {
            Debug.Log("Cheat mode");
            GameManager.instance.SetState(GameState.CheatMode);
        };
    }

    public void Buy(VitrineSlot slot) {
        if (slot.slot == null || slot.slot.item == null) return;
        if (slot.slot.quantidade <= 0 && !slot.slot.infinito) return;

        int preco = slot.slot.preco;
        if (Player.Atributos.pecas.Get() >= preco){
            Player.Atributos.pecas.Sub(preco);
            Player.instance.inventario.AddItem(slot.slot.item);
            slot.slot.RemoveItem();
        }
    }

    public void EntrouLoja() {
        Player.instance.gameObject.SetActive(false);

        // Enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        oldCam = Camera.main;
        oldCam.gameObject.SetActive(false);
        lojaCam.gameObject.SetActive(true);
        animator.SetTrigger(animatorTriggerName);

        dentro = true;

        GameManager.instance.SetState(GameState.Loja);
        GameManager.instance.controls.Loja.Sair.performed += SaiuLoja;
    }

    public void SaiuLoja(InputAction.CallbackContext ctx) {
        SaiuLoja();
    }

    public void SaiuLoja() {
        Player.instance.gameObject.SetActive(true);

        // Disable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (oldCam != null) oldCam.gameObject.SetActive(true);
        lojaCam.gameObject.SetActive(false);

        dentro = false;

        GameManager.instance.SetState(GameState.Playing);
        GameManager.instance.controls.Loja.Sair.performed -= SaiuLoja;
    }

    public void InterageComLoja() {
        if (dentro) SaiuLoja();
        else EntrouLoja();
    }

    public JSONObject Save() {
        JSONObject obj = new JSONObject();

        foreach (VitrineSlot vitrine in slots) {
            OfertaSlot slot = vitrine.slot;
            if (slot.infinito) continue;
            obj.AddField(slot.item.nome, slot.quantidade);
        }

        return obj;
    }

    public void Load(JSONObject obj) {
        if (obj == null) return;

        foreach (VitrineSlot vitrine in slots) {
            OfertaSlot slot = vitrine.slot;
            if (slot.item == null || slot.infinito) continue;
            if (obj.HasField(slot.item.nome)) {
                int quantidade = obj.GetField(slot.item.nome).intValue;
                slot.quantidade = quantidade;

                if (quantidade <= 0) {
                    if (vitrine != null && vitrine.gameObject != null)
                        vitrine.HandleEsgotado();
                }
            }
        }
    }
}
