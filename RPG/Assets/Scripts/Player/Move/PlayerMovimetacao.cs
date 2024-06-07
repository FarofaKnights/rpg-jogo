using UnityEngine;

public class PlayerMovimentacao : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rollSpeed = 15f;
    public float gravity = -9.81f;
    public float rollDuration = 0.5f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public float rotationSpeed = 5f;
    public float backRotationSpeed = 2f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool rolamento = false;
    private float rollTimer;
    private bool invulnerable = false;
    private bool canMove = true;
    private Transform mainCameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCameraTransform = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveSideways = transform.right * moveX * walkSpeed;

        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0f;

        Vector3 moveForward = cameraForward.normalized * moveZ * walkSpeed;

        Vector3 move = moveSideways + moveForward;

        animator.SetFloat("inputX", moveX);
        animator.SetFloat("inputZ", moveZ);

        if (!rolamento && move != Vector3.zero && !Input.GetKey(KeyCode.S) && canMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward.normalized);
            float currentRotationSpeed = (moveZ < 0) ? backRotationSpeed : rotationSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
        }

        if (rolamento)
        {
            Roll();
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Run(move);
            }
            else
            {
                Walk(move);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !rolamento)
            {
                StartRoll(move);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    void Walk(Vector3 move)
    {
        if (canMove)
            controller.Move(move * Time.deltaTime);
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
    public bool IsInvulnerable()
    {
        return invulnerable;
    }
}
