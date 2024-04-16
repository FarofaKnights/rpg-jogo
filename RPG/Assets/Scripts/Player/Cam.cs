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
    Vector3 cameraObjectPosition;
    float cameraSmoothSpeed = 1;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    [SerializeField] float minimunPivot = -30;
    [SerializeField] float maximunPivot = 60;
    [SerializeField] Transform cameraPivotTransform;
    [SerializeField] LayerMask collideWithLayer;

    [SerializeField] float cameraCollisionRadius = 0.2f;
    float cameraZPosition;
    float targetCameraZPosition;


    private void Awake()
    {
        instance = this;
        cameraZPosition = cameraObject.transform.localPosition.z;
    }
    public void CameraActions()
    {
        if(player != null) 
        {
            FollowTarget();
            Rotations();
            Colisoes();
        }
    }
    void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;

    }
    void Rotations()
    {
        leftAndRightLookAngle += (player.mouseX * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (player.mouseY * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimunPivot, maximunPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;

    }
    void Colisoes()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayer))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
