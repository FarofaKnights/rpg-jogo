using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoTeleporta : Acao {
    public string idRef;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id"}; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id do RefObject"}; }

    public AcaoTeleporta(string idRef, string funcaoName) {
        this.idRef = idRef;
    }

    public AcaoTeleporta(AcaoParams parametros): base(parametros) {
        idRef = parametros.id;
    }

    public override void Realizar() {
        RefObject[] refObjects = GameManager.instance.GetObjectsOfType<RefObject>(true);
        foreach (RefObject refObj in refObjects) {
            if (refObj.id == idRef) {
                Player.instance.TeleportTo(refObj.transform.position);
                break;
            }
        }
    }

}
