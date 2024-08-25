using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAlternativa : MonoBehaviour {
    public float sensibilidade = 1;
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    Vector2 rot;

    void Update() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rot.x += mouseX * sensibilidade * Time.deltaTime;
        rot.y += mouseY * sensibilidade * Time.deltaTime;

        rot.y = Mathf.Clamp(rot.y, -90f, 90f);

        transform.eulerAngles = new Vector3(-rot.y, rot.x, 0);
        transform.position = target.position - transform.forward * offset.z + transform.up * offset.y + transform.right * offset.x;
    }
}
