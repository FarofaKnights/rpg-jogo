using UnityEngine;
using Cinemachine;

public class PlayerMovimentacao : MonoBehaviour
{
    [HideInInspector] public bool isWalkState = false;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rollSpeed = 15f;
    public float gravity = -9.81f;
    public float rollDuration = 0.5f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public float rotationSpeed = 5f;
    public float backRotationSpeed = 2f;
    public float lockOnRadius = 10f;
    public KeyCode lockOnKey = KeyCode.F;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool rolamento = false;
    private float rollTimer;
    private bool invulnerable = false;
    private bool canMove = true;
    private Transform mainCameraTransform;
    public Transform lockedTarget;
    private CinemachineFreeLook cinemachineFreeLook;
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCameraTransform = Camera.main.transform;
        cinemachineFreeLook = FindObjectOfType<CinemachineFreeLook>();
        virtualCamera = cinemachineFreeLook.GetComponent<CinemachineVirtualCamera>();

        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.Follow = transform;
            cinemachineFreeLook.LookAt = transform.Find("Look");
        }
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        MovementControl();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("IsGrounded", isGrounded);        
    }
    private void FixedUpdate()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
            if (lockedTarget != null)
            {
                virtualCamera.LookAt = lockedTarget;
            }
        }
    }

    void MovementControl()
    {
        if (!isWalkState)  return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (moveX == 0 && moveZ == 0)
        {
            animator.SetFloat("inputX", 0);
            animator.SetFloat("inputZ", 0);
            animator.SetBool("Correr", false);
            AudioManager.instance.playerFootsteps.Pause();
            return;
        }
        AudioManager.instance.playerFootsteps.UnPause();
        Vector3 moveSideways = transform.right * moveX;

        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0f;

        Vector3 moveForward = cameraForward.normalized * moveZ;

        Vector3 move = moveSideways + moveForward;

        animator.SetFloat("inputX", moveX);
        animator.SetFloat("inputZ", moveZ);

        if (rolamento)
        {
            Roll();
        }
        else
        {
            if (!Input.GetKey(KeyCode.S) && canMove && move != Vector3.zero)
            {
                if (lockedTarget != null)
                {
                    Vector3 directionToTarget = (lockedTarget.position - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(directionToTarget, Vector3.up));
                    float currentRotationSpeed = (moveZ < 0) ? backRotationSpeed : rotationSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cameraForward.normalized);
                    float currentRotationSpeed = (moveZ < 0) ? backRotationSpeed : rotationSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
                }
            }

            if (Input.GetKeyDown(lockOnKey))
            {
               // LockOn();
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Walk(move);
            }
            else
            {
                Run(move);
            }
            if (Input.GetKeyDown(KeyCode.Space) && !rolamento)
            {
                StartRoll(move);
            }
        }
    }

    void Walk(Vector3 move)
    {
        if (canMove)
            controller.Move(move * walkSpeed * Time.deltaTime);
        animator.SetBool("Correr", false);
    }

    void Run(Vector3 move)
    {
        if (canMove)
            controller.Move(move * runSpeed * Time.deltaTime);
        animator.SetBool("Correr", true);
    }

    void StartRoll(Vector3 move)
    {
        if (move.magnitude != 0)
        {
            rolamento = true;
            rollTimer = rollDuration;
            Vector3 rollDirection = move.normalized;
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
        }
        else
        {
            rolamento = true;
            rollTimer = rollDuration;
            Vector3 rollDirection = mainCameraTransform.forward.normalized;
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
        }
        animator.SetBool("Rolamento", true);
        invulnerable = true;
        canMove = false;
    }

    void Roll()
    {
        if (rollTimer > 0)
        {
            controller.Move(transform.forward * rollSpeed * Time.deltaTime);
            rollTimer -= Time.deltaTime;
        }
        else
        {
            rolamento = false;
            animator.SetBool("Rolamento", false);
            invulnerable = false;
            canMove = true;
        }
    }

    /*void LockOn()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnRadius);
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Inimigo"))
            {
                Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToTarget, out hit, lockOnRadius, ~groundLayer))
                {
                    if (!hit.collider.CompareTag("Inimigo"))
                        continue;
                }
                float distanceToTarget = Vector3.Distance(transform.position, col.transform.position);
                if (distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    nearestTarget = col.transform;
                }
            }
        }

        if (nearestTarget != null)
        {
            lockedTarget = nearestTarget;
        }
        else
        {
            lockedTarget = null;
        }
    }
    */
    public bool IsInvulnerable()
    {
        return invulnerable;
    }

    public void IsWalking(bool isWalking)
    {
        isWalkState = isWalking;

        if (!isWalkState)
        {
            animator.SetFloat("inputX", 0);
            animator.SetFloat("inputZ", 0);
        }
    }
}
