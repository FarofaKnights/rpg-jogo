using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovementInfo {
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rollSpeed = 15f;
    public float gravity = -9.81f;
    public float rollDuration = 0.5f;
    public LayerMask groundLayer;
    public float rotationSpeed = 5f;
    public float backRotationSpeed = 2f;
    public KeyCode lockOnKey = KeyCode.F;
}
