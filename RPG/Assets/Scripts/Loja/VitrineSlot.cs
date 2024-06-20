using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VitrineSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public OfertaSlot slot;

    [Header("UI")]
    public GameObject indicativo;
    public Text titulo;
    public Text descricao;
    public Text preco;

    bool mouseOver = false;

    void Start() {
        slot.OnDelete += HandleEsgotado;
        UpdateUI();

        indicativo.SetActive(false);
    }

    public void UpdateUI() {
        titulo.text = slot.item.nome;
        descricao.text = slot.item.descricao;
        preco.text = slot.preco.ToString() + " peças";

        RectTransform root = indicativo.GetComponentInChildren<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        StartCoroutine(UpdateLayoutGroup());
    }

    IEnumerator UpdateLayoutGroup() {
        descricao.transform.parent.gameObject.GetComponent<LayoutGroup>().enabled = false;
        yield return new WaitForEndOfFrame();
        descricao.transform.parent.gameObject.GetComponent<LayoutGroup>().enabled = true;
    }

    public void HandleEsgotado() {
        if (slot.quantidade <= 0 && !slot.infinito) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            Collider col = GetComponent<Collider>();
            if (col != null) {
                Destroy(col);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        LojaController controller = GetComponentInParent<LojaController>();
        if (controller == null) return;
        controller.Buy(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        indicativo.SetActive(true);
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        indicativo.SetActive(false);
        mouseOver = false;
    }
}
