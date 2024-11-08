using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : IPlayerState {
    public Player player;
    Animator animator;
    Transform aimLook;
    PlayerAimInfo info;
    CharacterController controller;
    bool aimingAtEnemy = false;
    bool firstRun = false;
    float distanciaMaxTiro = 100f;
    Vector3 screenCenter;

    public PlayerAimState(Player player) {
        this.player = player;
        info = player.informacoesMira;
        controller = player.GetComponent<CharacterController>();
        aimLook = player.aimLook.transform;
        animator = player.animator;
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    public void Enter() {
        player.RotatePlayerToCamera();
        player.aimCam.Priority = 11;
        player.thirdPersonCam.Priority = 9;
        UIController.HUD.ShowAim(true);

        animator.SetBool("Correr", false);
        animator.SetFloat("inputX", 0);
        animator.SetFloat("inputZ", 0);

        if (player.braco.GetType() == typeof(BracoShooter)) {
            BracoShooter braco = (BracoShooter)player.braco;
            distanciaMaxTiro = braco.GetMaxDistance();
        }

        firstRun = true;
    }

    public void Execute() {
        float rotX = GameManager.instance.controls.Player.Camera.ReadValue<Vector2>().x;
        float rotY = GameManager.instance.controls.Player.Camera.ReadValue<Vector2>().y;

        rotY = Mathf.Clamp(rotY, -info.yRotationLimit, info.yRotationLimit);

        float moveX = GameManager.instance.controls.Player.Movement.ReadValue<Vector2>().x;
        float moveY = GameManager.instance.controls.Player.Movement.ReadValue<Vector2>().y;

        Vector3 move = player.transform.right * moveX + player.transform.forward * moveY;

        player.transform.Rotate(Vector3.up, rotX * Time.fixedDeltaTime * info.xRotationSpeed);
        aimLook.Rotate(Vector3.right, -rotY * Time.fixedDeltaTime * info.yRotationSpeed);
        
        float aimLookRotation = aimLook.localRotation.eulerAngles.x;
        if (aimLookRotation > info.yRotationLimit && aimLookRotation < 180) {
            aimLook.localRotation = Quaternion.Euler(info.yRotationLimit, aimLook.localRotation.eulerAngles.y, aimLook.localRotation.eulerAngles.z);
        } else if (aimLookRotation < 360 - info.yRotationLimit && aimLookRotation > 180) {
            aimLook.localRotation = Quaternion.Euler(360 - info.yRotationLimit, aimLook.localRotation.eulerAngles.y, aimLook.localRotation.eulerAngles.z);
        }

        controller.Move(move * info.walkSpeed * Time.fixedDeltaTime);
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);


        // Aiming at enemy
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        bool currentlyAimingAtEnemy = false;

        // O raio neste caso é calculado da câmera para o objeto observado, então para descobrir a distância maxima do tiro fazemos:
        // - Um triangulo entre a câmera, o maximo e a origem do tiro

        // Calculamos o angulo entre o player, a camera e o maximo, com a camera sendo o ponto de origem
        Vector3 player2cam = Camera.main.transform.position - player.transform.position;
        float camPlayerAngle = Vector3.Angle(player2cam.normalized, ray.direction);

        
        // Assim, a distancia maxima é a hipotenusa, portanto:
        // hipotenusa = oposto / seno(angulo) => distancia = distanciaMaxTiro / seno(angulo)
        // Estranhamente ainda não tá 100% certo, mas tá bem proximo
        float distancia = distanciaMaxTiro / Mathf.Sin(camPlayerAngle * Mathf.Deg2Rad);

        if (Physics.Raycast(ray, out hit, distancia, info.layerMask)) {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            currentlyAimingAtEnemy = true;
        }

        if (currentlyAimingAtEnemy != aimingAtEnemy) {
            aimingAtEnemy = currentlyAimingAtEnemy;
            UIController.HUD.AimHasTarget(aimingAtEnemy);
        }
    }

    public void Update() {
        // Atirar no modo mira
        if (Input.GetMouseButtonDown(0)) {
            if (player.braco != null) {
                player.braco.Ativar();
            }
        }

        // Sair do modo tiro
        if (Input.GetMouseButtonDown(1) && !firstRun) {
            CallExit();
        }

        firstRun = false;
    }

    void CallExit() {
        player.canChangeStateThisFrame = false;
        player.stateMachine.SetState(player.moveState);
    }

    public void Exit() {
        player.aimCam.Priority = 9;
        player.thirdPersonCam.Priority = 11;
        UIController.HUD.ShowAim(false);
        Vector3 aimCamRot = player.aimCam.transform.rotation.eulerAngles;
        player.thirdPersonCam.transform.rotation = Quaternion.Euler(0, aimCamRot.y, 0);
    }
}
