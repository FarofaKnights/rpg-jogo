using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraLoja : MonoBehaviour {
    Camera oldCam;
    public Camera lojaCam;
    public string triggerName;
    public Animator animator;
    bool dentro = false;
    
    public void OnPlayerInteract() {
        Debug.Log("Dentro: " + dentro);

        if (!dentro) {
            oldCam = Camera.main;
            oldCam.gameObject.SetActive(false);
            lojaCam.gameObject.SetActive(true);
            animator.SetTrigger(triggerName);
        } else {
            oldCam.gameObject.SetActive(true);
            lojaCam.gameObject.SetActive(false);
        }

        dentro = !dentro;
    }
}
