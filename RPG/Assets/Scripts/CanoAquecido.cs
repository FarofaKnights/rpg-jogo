using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanoAquecido : MonoBehaviour
{
    public GameObject Fogo;
    public GameObject Vapor;
    Color lerpedColor = Color.white;
    float cor1 = 0.5F;
    bool Morte = false;
    Color orange = new Color(1.0f, 0.64f, 0, 255);
    Color brown = new Color(0.3f, 0.2f, 0.09f, 255 );

    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        StartCoroutine(DangerCountdown());
      
    }

    void OnColisionEnter(Collider other)
    {
        Debug.Log("Coisinha coisada");
        if (Morte == true)
        {

            Debug.Log("Coisa");

            if (other.CompareTag("Player"))
            {
                Debug.Log("Playyeyey");
                Destroy(other.gameObject);
                SceneManager.LoadScene("GameOverTEMP");
            }

        }

    }
    IEnumerator DangerCountdown()
    {
        yield return new WaitForSecondsRealtime(5);
        DangerState();
    }

    void DangerState()
    {
        Debug.Log("DangerState");
        StartCoroutine(ChangeColor(renderer.material.color, orange,.005f));
        Vapor.SetActive(true);
        StartCoroutine(DeathCountdown());
    }
    IEnumerator DeathCountdown()
    {
        yield return new WaitForSecondsRealtime(3);
        DeathState();
    }
    void DeathState()
    {
        Debug.Log("DeathState");
        Vapor.SetActive(false);
        StartCoroutine(ChangeColor(renderer.material.color, Color.red,.005f));
        Fogo.SetActive(true);
        Morte = true;
        StartCoroutine(SafeCountdown());
    }
    IEnumerator SafeCountdown()
    {
        yield return new WaitForSecondsRealtime(5);
        SafeState();
    }
    void SafeState()
    {
        Debug.Log("SafeState");
        Morte = false;
        Fogo.SetActive(false);
        StartCoroutine(ChangeColor(renderer.material.color,brown,.005f));
        StartCoroutine(DangerCountdown());
    }

    IEnumerator ChangeColor(Color start, Color destiny, float time)
    {    
        yield return new WaitForEndOfFrame();

        lerpedColor = Color.Lerp(start, destiny, time);
        renderer.material.color = lerpedColor;                

        if(lerpedColor != destiny){
            StartCoroutine(ChangeColor(lerpedColor,destiny,time));
        }    

    }

    
}
