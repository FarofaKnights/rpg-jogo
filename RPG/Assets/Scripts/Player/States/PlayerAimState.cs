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

    public PlayerAimState(Player player) {
        this.player = player;
        info = player.informacoesMira;
        controller = player.GetComponent<CharacterController>();
        aimLook = player.aimLook.transform;
        animator = player.animator;
    }

    public void Enter() {
        player.aimCam.Priority = 11;
        player.thirdPersonCam.Priority = 9;
        UIController.HUD.ShowAim(true);

        animator.SetBool("Correr", false);
        animator.SetFloat("inputX", 0);
        animator.SetFloat("inputZ", 0);
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
    }

    public void Update() {
        // Atirar no modo mira
        if (Input.GetMouseButtonDown(0)) {
            if (player.braco != null) {
                player.braco.Ativar();

                if (!player.braco.PodeAtivar()) CallExit();
            }
        }

        // Sair do modo tiro
        if (Input.GetMouseButtonDown(1)) {
            CallExit();
        }

        // Get middle screen position
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        bool currentlyAimingAtEnemy = false;
        if (Physics.Raycast(ray, out hit, 100, info.layerMask)) {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            currentlyAimingAtEnemy = true;
        }

        if (currentlyAimingAtEnemy != aimingAtEnemy) {
            aimingAtEnemy = currentlyAimingAtEnemy;
            UIController.HUD.AimHasTarget(aimingAtEnemy);
        }
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
