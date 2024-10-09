using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AcaoChangeCam : Acao {
    public string idCamera;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id da Camera" }; }

    public AcaoChangeCam(string idCamera) {
        this.idCamera = idCamera;
    }

    public AcaoChangeCam(AcaoParams parametros): base(parametros) {
        idCamera = parametros.id;
    }

    public override void Realizar() {
        if (idCamera == "" || idCamera == null) {
            UIController.dialogo.SetCamera(null);
            return;
        }

        CameraObject[] cameraObjects = GameManager.instance.GetObjectsOfType<CameraObject>(true);
        foreach (CameraObject cameraObject in cameraObjects) {
            if (cameraObject.cameraId == idCamera) {
                CinemachineVirtualCamera cam = cameraObject.GetComponent<CinemachineVirtualCamera>();
                UIController.dialogo.SetCamera(cam);
                return;
            }
        }
    }

}
