using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : IPlayerState {
    public Player player;

    public PlayerAimState(Player player) {
        this.player = player;
    }

    public void Enter() {
        player.aimCam.gameObject.SetActive(true);
        player.thirdPersonCam.gameObject.SetActive(false);
    }

    public void Execute() {
    }

    public void Exit() {
        player.aimCam.gameObject.SetActive(false);
        player.thirdPersonCam.gameObject.SetActive(true);
    }
}
