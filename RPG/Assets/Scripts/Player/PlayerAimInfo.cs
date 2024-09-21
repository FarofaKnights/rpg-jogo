using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAimInfo {
    public float xRotationSpeed = 30f;
    public float yRotationSpeed = 15f;
    public float walkSpeed = 3f;
    public float yRotationLimit = 45f;
    public LayerMask layerMask;
}
