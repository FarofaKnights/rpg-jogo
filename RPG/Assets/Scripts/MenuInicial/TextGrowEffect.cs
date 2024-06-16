using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform visualRect;
    private Vector3 initialScale;
    public float hoverScaleFactor = 1.7f;
    public float scaleSpeed = 10f;

    private bool isHovering = false;

    void Start()
    {
        // Encontrar o RectTransform do GameObject Visual (que é um filho do botão)
        visualRect = transform.GetComponent<RectTransform>();

        // Salvar a escala inicial
        initialScale = visualRect.localScale;
    }

    void Update()
    {
        // Verificar se estamos em hover
        if (isHovering)
        {
            // Interpolar suavemente entre o tamanho atual e o tamanho aumentado
            visualRect.localScale = Vector3.Lerp(visualRect.localScale, initialScale * hoverScaleFactor, Time.deltaTime * scaleSpeed);
        }
        else
        {
            // Interpolar suavemente de volta ao tamanho inicial
            visualRect.localScale = Vector3.Lerp(visualRect.localScale, initialScale, Time.deltaTime * scaleSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
