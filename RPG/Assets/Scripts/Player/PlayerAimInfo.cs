using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAimInfo {
    public float walkSpeed = 3f;
    public float turnSpeed = 5f;
    public float sensitivity = 1.5f;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    public LayerMask layerMask;
}
