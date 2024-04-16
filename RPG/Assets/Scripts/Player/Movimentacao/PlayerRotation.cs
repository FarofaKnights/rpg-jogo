using UnityEngine;
using Cinemachine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float sensitivy = 150f;
    private float rotationX;
    public float mouseX, mouseY;
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        RotatePlayer();
        //Cam.instance.CameraActions();
    }

    private void RotatePlayer()
    {
        mouseX = Input.GetAxis("Mouse X") ;
        mouseY = Input.GetAxis("Mouse Y");
        rotationX += mouseX * sensitivy * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotationX, 0);        
    }
}
