using UnityEngine;
using Cinemachine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float sensitivy = 150f;
    private float rotationX;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") ;
        rotationX += mouseX * sensitivy * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotationX, 0);        
    }
}
