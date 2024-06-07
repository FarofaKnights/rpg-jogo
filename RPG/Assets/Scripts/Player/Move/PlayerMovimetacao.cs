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
    private bool canChangeRollDirection = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        animator.SetFloat("inputX", moveX);
        animator.SetFloat("inputZ", moveZ);

        if (!rolamento && move != Vector3.zero && !Input.GetKey(KeyCode.S))
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
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
                Run(move, moveZ);
            }
            else
            {
                Walk(move);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartRoll();
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    void Walk(Vector3 move)
    {
        controller.Move(move * walkSpeed * Time.deltaTime);
        animator.SetBool("Correr", false);
    }

    void Run(Vector3 move, float moveZ)
    {
        controller.Move(move * runSpeed * Time.deltaTime);
        animator.SetBool("Correr", true);
    }

    void StartRoll()
    {
        if (!rolamento)
        {
            rolamento = true;
            canChangeRollDirection = false;
            rollTimer = rollDuration;

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 rollDirection = Vector3.zero;

            if (verticalInput < 0)
            {
                rollDirection += -transform.forward;
            }
            if (verticalInput > 0)
            {
                rollDirection += transform.forward;
            }
            if (horizontalInput < 0)
            {
                rollDirection += -transform.right;
            }
            if (horizontalInput > 0)
            {
                rollDirection += transform.right;
            }

            if (rollDirection.magnitude > 0)
            {
                rollDirection.Normalize();
            }
            else
            {
                rollDirection = transform.forward;
            }

            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            animator.SetBool("Rolamento", true);
        }
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
            canChangeRollDirection = true;
            animator.SetBool("Rolamento", false);
        }
    }
}
