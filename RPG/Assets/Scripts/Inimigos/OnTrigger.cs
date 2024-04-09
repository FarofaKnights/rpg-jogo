using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : MonoBehaviour {
    public string tagFilter = "";
    public System.Action<GameObject> onTriggerEnter, onTriggerExit;

    void OnTriggerEnter(Collider other) {
        if (tagFilter != "" && !other.CompareTag(tagFilter)) return;
        
        onTriggerEnter?.Invoke(other.gameObject);
    }

    void OnTriggerExit(Collider other) {
        if (tagFilter != "" && !other.CompareTag(tagFilter)) return;

        onTriggerExit?.Invoke(other.gameObject);
    }
}
