using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cam : MonoBehaviour
{
    public static Cam instance;
    public PlayerRotation player;
    public Camera cameraObject;

    [Header("Configuracoes da Camera")]
    Vector3 cameraVelocity;
    float cameraSmoothSpeed = 1;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    [SerializeField] float minimunPivot = -30;
    [SerializeField] float maximunPivot = 60;
    [SerializeField] Transform cameraPivotTransform;

    private void Awake()
    {
        instance = this;
    }
    public void CameraActions()
    {
        if(player != null) 
        {
            FollowTarget();
            Rotations();
        }
    }
    void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity,cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;

    }
    void Rotations()
    {
        leftAndRightLookAngle += (player.mouseX * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle += (player.mouseY * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimunPivot, maximunPivot);
        Vector3 cameraRotation = Vector3.zero;
        cameraRotation.y = leftAndRightLookAngle;
        Quaternion targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        transform.rotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;

    }
}
