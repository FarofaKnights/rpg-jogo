using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControle : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float sensitivity = 10f;
    public float maxYAngle = 80f;

    private Vector2 currentRotation;

    void Start()
    {
        currentRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

   
        Quaternion camRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        transform.position = player.position + camRotation * offset;
        transform.LookAt(player.position + Vector3.up * 1.5f);  
    }
}