using UnityEngine;
using Cinemachine;

public class LockOnTarget : MonoBehaviour
{
    public Transform player;
    public string enemyTag = "Inimigo";
    public Transform focusPoint;
    public float switchCooldown = 0.5f;
    public float focusPointLerpSpeed = 5f;
    public float maxLockOnDistance = 10f;
    public Transform defaultFocusParent;
    public Transform lockOnFocusParent;

    private CinemachineFreeLook freeLookCamera;
    private Transform currentTarget;
    private float lastSwitchTime;
    private bool isLockedOn = false;
    private Vector3 originalFocusPointLocalPosition;

    void Start() {
        if (player == null)  player = Player.instance.transform;
        if (focusPoint == null) focusPoint = player.Find("Look");
        if (defaultFocusParent == null) defaultFocusParent = player.Find("LookInicial");
        
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        originalFocusPointLocalPosition = focusPoint.localPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (isLockedOn)
            {
                UnlockTarget();
            }
            else
            {
                SwitchTarget();
            }
        }

        if (isLockedOn && currentTarget != null)
        {
            LockOnToTarget();
        }
       
    }

    void SwitchTarget()
    {
        if (Time.time - lastSwitchTime < switchCooldown)
            return;

        lastSwitchTime = Time.time;

        Transform closestTarget = FindClosestTarget();
        if (closestTarget != null)
        {
            float distanceToTarget = Vector3.Distance(player.position, closestTarget.position);
            if (distanceToTarget <= maxLockOnDistance)
            {
                currentTarget = closestTarget;
                isLockedOn = true;
                focusPoint.SetParent(lockOnFocusParent);
            }
        }
    }

    void UnlockTarget()
    {
        currentTarget = null;
        isLockedOn = false;
        focusPoint.SetParent(defaultFocusParent);
        focusPoint.localPosition = originalFocusPointLocalPosition;
    }

    void LockOnToTarget()
    {
        Vector3 directionToTarget = currentTarget.position - player.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 5f);

        Vector3 midpoint = (player.position + currentTarget.position) / 2;
        focusPoint.position = Vector3.Lerp(focusPoint.position, midpoint, Time.deltaTime * focusPointLerpSpeed);

    }

    Transform FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToPlayer = Vector3.Distance(player.position, enemy.transform.position);
            if (distanceToPlayer < closestDistance && distanceToPlayer <= maxLockOnDistance)
            {
                closestDistance = distanceToPlayer;
                closestTarget = enemy.transform;
            }
        }

        return closestTarget;
    }
}
