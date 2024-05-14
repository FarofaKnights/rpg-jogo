using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    InputHandler inputHandler;
    public Transform targetTransform;
    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    public Transform leftLockTarget;
    public Transform RightLockTarget;
    public Transform currentLockOnTarget;
    public Transform nearestLockOnTarget;



    List<CharacterManager> avaliableTargets = new List<CharacterManager>();
    public float maximumLockOnDistance = 30;


    private void Awake()
    {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        inputHandler = FindAnyObjectByType<InputHandler>();

        cameraPivotTransform = transform.GetChild(0);
        cameraTransform = cameraPivotTransform.GetChild(0);
    }
    public void FollowTarget(float delta)
    {
        Vector3 targerPosition = Vector3.SmoothDamp
            (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targerPosition;

        HandleCameraCollision(delta);
    }
    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        if (inputHandler.LockOnFlag == false && currentLockOnTarget == null)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotAngle) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targerRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targerRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targerRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targerRotation;
        }
        else
        {          
            Vector3 dir = currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.position - cameraPivotTransform.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivotTransform.localEulerAngles = eulerAngle;
        }

    }
    private void HandleCameraCollision(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direciton = cameraTransform.position - cameraPivotTransform.position;
        direciton.Normalize();
        if(Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direciton, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }
        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }
        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;
    }
    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
        for(int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();
            if(character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                if (character.transform.root != targetTransform.transform.root
                    && viewableAngle > -50 && viewableAngle < 50 
                    && distanceFromTarget <= maximumLockOnDistance)
                {
                    avaliableTargets.Add(character);
                }               
            }
        }

        for(int k = 0; k < avaliableTargets.Count; k++)
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, avaliableTargets[k].transform.position);
            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = avaliableTargets[k].lockOnTransform;
            }

            if (inputHandler.LockOnFlag)
            {
                Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(avaliableTargets[k].transform.position);
                var distanceFromLeftTargert = currentLockOnTarget.transform.position.x - avaliableTargets[k].transform.position.x;
                var distanceFromRightTargert = currentLockOnTarget.transform.position.x + avaliableTargets[k].transform.position.x;
                
                if(relativeEnemyPosition.x > 0.00 && distanceFromLeftTargert < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTargert;
                    leftLockTarget = avaliableTargets[k].lockOnTransform;
                }
                if(relativeEnemyPosition.x < 0.00 && distanceFromRightTargert < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTargert;
                    RightLockTarget = avaliableTargets[k].lockOnTransform;
                }
            }
        }
    }
    public void ClearLockTargets()
    {
        avaliableTargets.Clear();
        currentLockOnTarget = null;
        nearestLockOnTarget = null;
    }
}
