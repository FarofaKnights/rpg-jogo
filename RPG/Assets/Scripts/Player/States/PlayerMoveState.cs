using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMoveState : IPlayerState {
    public Player player;

    // Atributos
    bool isGrounded;
    bool canMove = true;
    Transform lockedTarget;


    // Referências
    StateMachine<IPlayerState> stateMachine;
    PlayerMovementInfo info;
    CharacterController controller;
    Transform mainCameraTransform;
    CinemachineVirtualCamera virtualCamera;
    Animator animator;
    PossuiVida vidaPlayer;


    public PlayerMoveState(Player player) {
        this.player = player; 
    }

    public void Enter() {
        player.aimCam.Priority = 9;
        player.thirdPersonCam.Priority = 11;

        animator = player.animator;
        info = player.informacoesMovimentacao;
        stateMachine = player.stateMachine;
        controller = player.GetComponent<CharacterController>();
        vidaPlayer = player.GetComponent<PossuiVida>();
        mainCameraTransform = Camera.main.transform;
        virtualCamera = player.thirdPersonCam.GetComponent<CinemachineVirtualCamera>();
    }

    public void Update() {
        MovementControl();
    }

    public void Execute() {
        if (Time.timeScale == 0 && AudioManager.instance.playerFootsteps.isPlaying) {
            AudioManager.instance.playerFootsteps.Pause();
        }
    }

    public void Exit() {
        vidaPlayer.SetInvulneravel(false);
        AudioManager.instance.playerFootsteps.Pause();
    }

    void MovementControl() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (GameManager.instance.state != GameState.Playing) {
            moveX = 0;
            moveZ = 0;
        }

        // Se não estiver se movendo
        if (moveX == 0 && moveZ == 0) {
            animator.SetFloat("inputX", 0);
            animator.SetFloat("inputZ", 0);
            AudioManager.instance.playerFootsteps.Pause();
            return;
        }

        AudioManager.instance.playerFootsteps.UnPause();
        Vector3 moveSideways = player.transform.right * moveX;

        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0f;

        Vector3 moveForward = cameraForward.normalized * moveZ;

        Vector3 move = moveSideways + moveForward;

        animator.SetFloat("inputX", moveX);
        animator.SetFloat("inputZ", moveZ);

        if (!Input.GetKey(KeyCode.S) && canMove && move != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward.normalized);
            float currentRotationSpeed = (moveZ < 0) ? info.backRotationSpeed : info.rotationSpeed;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift) || player.GetDanoFrioValue() > 0) Walk(move);
        else Run(move);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Dash();
        }
    }

    void Walk(Vector3 move) {
        if (canMove)
            controller.Move(move * info.walkSpeed * Time.deltaTime);
        animator.SetBool("Correr", false);
    }

    void Run(Vector3 move) {
        if (canMove)
            controller.Move(move * info.runSpeed * Time.deltaTime);
        animator.SetBool("Correr", true);
    }

    public void Dash() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        player.velocity = new Vector3(moveX, 0, moveZ);
        stateMachine.SetState(player.dashState);
    }
}