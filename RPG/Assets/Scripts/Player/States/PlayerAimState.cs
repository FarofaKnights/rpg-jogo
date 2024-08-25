using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : IPlayerState {
    public Player player;
    Animator animator;
    Transform aimLook;
    PlayerAimInfo info;
    CharacterController controller;

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

        player.transform.Rotate(Vector3.up, rotX * Time.deltaTime * info.xRotationSpeed);
        aimLook.Rotate(Vector3.right, -rotY * Time.deltaTime * info.yRotationSpeed);

        controller.Move(move * info.walkSpeed * Time.deltaTime);

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
    }

    void CallExit() {
        player.canChangeStateThisFrame = false;
        player.stateMachine.SetState(player.moveState);
    }

    public void Exit() {
        player.aimCam.Priority = 9;
        player.thirdPersonCam.Priority = 11;
        Vector3 aimCamRot = player.aimCam.transform.rotation.eulerAngles;
        player.thirdPersonCam.transform.rotation = Quaternion.Euler(0, aimCamRot.y, 0);
    }
}
