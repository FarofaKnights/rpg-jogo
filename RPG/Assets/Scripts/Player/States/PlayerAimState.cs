using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : IPlayerState {
    public Player player;
    Animator animator;
    Transform aimLook;

    public PlayerAimState(Player player) {
        this.player = player;
    }

    public void Enter() {
        player.aimCam.gameObject.SetActive(true);
        player.thirdPersonCam.gameObject.SetActive(false);

        animator = player.animator;
        animator.SetBool("Correr", false);
        animator.SetFloat("inputX", 0);
        animator.SetFloat("inputZ", 0);

        aimLook = player.aimLook.transform;

    }

    public void Execute() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log(moveX);
        aimLook.Rotate(Vector3.up, moveX * Time.deltaTime * 100);
    }

    public void Exit() {
        player.aimCam.gameObject.SetActive(false);
        player.thirdPersonCam.gameObject.SetActive(true);
    }
}
