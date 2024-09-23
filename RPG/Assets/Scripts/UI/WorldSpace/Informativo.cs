using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Informativo : MonoBehaviour {
    public Vector3 offset;
    public GameObject informativo;

    void Awake() {
        GameObject prefab = Resources.Load<GameObject>("UI/Informativo");
        informativo = Instantiate(prefab, transform);

        informativo.transform.SetParent(null);
        informativo.transform.localScale = prefab.transform.localScale;
        informativo.transform.SetParent(transform);
        informativo.SetActive(false);

        informativo.transform.position += offset;
    }

    void OnDrawGizmosSelected() {
        GameObject prefab = Resources.Load<GameObject>("UI/Informativo");
        if (prefab == null) return;

        Gizmos.color = Color.green;
        Vector3 bounds = prefab.GetComponent<BoxCollider>().size;
        bounds *= prefab.transform.localScale.x;
        Gizmos.DrawWireCube(transform.position + offset, bounds);
        
        //draw line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + offset);
    }
}
