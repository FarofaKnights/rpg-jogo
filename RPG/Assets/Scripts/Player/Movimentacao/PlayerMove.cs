using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speedPlayer;
    private CharacterController characterPlayer;
    private Animator animationPlayer;    
    private Vector3 movementPlayer;

    void Awake()
    {
        animationPlayer = GetComponent<Animator>();
        characterPlayer = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMovement();
        Gravity();
    }

    private void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementPlayer = (transform.right * horizontal) + (transform.forward * vertical);
        characterPlayer.Move(movementPlayer.normalized * speedPlayer * Time.deltaTime);
        PlayerAnimation(horizontal, vertical);
    }

    private void Gravity()
    {
        movementPlayer = Vector3.zero;
        if (characterPlayer.isGrounded == false)
        {
            movementPlayer += Physics.gravity; 
        }
        characterPlayer.Move(movementPlayer * Time.deltaTime);
    }

    private void PlayerAnimation(float x, float z)
    {
        animationPlayer.SetFloat("inputX", x);
        animationPlayer.SetFloat("inputZ", z);
    }
}
