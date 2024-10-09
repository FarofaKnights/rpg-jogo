using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraObject : MonoBehaviour
{
    [Header("Camera Referenciavel")]
    public string cameraId;
}
