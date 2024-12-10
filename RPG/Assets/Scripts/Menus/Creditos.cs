using UnityEngine;
using UnityEngine.UI;

public class Creditos : MonoBehaviour
{
    public Text creditsText;
    public float scrollSpeed = 20f; 
    public float acceleratedSpeed = 80f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = scrollSpeed;
    }

    void Update() {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            currentSpeed = acceleratedSpeed;
        } else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            currentSpeed = -acceleratedSpeed;
        } else {
            currentSpeed = scrollSpeed;
        }

        Vector2 newPosition = creditsText.rectTransform.anchoredPosition;
        newPosition.y += currentSpeed * Time.deltaTime;
        creditsText.rectTransform.anchoredPosition = newPosition;
    }
}
