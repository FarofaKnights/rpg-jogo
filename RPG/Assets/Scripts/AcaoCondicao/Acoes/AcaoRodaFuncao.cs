using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoRodaFuncao : Acao {
    public string idRef, funcaoName;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "stringValue"}; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id do RefObject", "Nome da Função" }; }

    public AcaoRodaFuncao(string idRef, string funcaoName) {
        this.idRef = idRef;
        this.funcaoName = funcaoName;
    }

    public AcaoRodaFuncao(AcaoParams parametros): base(parametros) {
        idRef = parametros.id;
        funcaoName = parametros.stringValue;
    }

    public override void Realizar() {
        RefObject[] refObjects = GameManager.instance.GetObjectsOfType<RefObject>(true);
        foreach (RefObject refObj in refObjects) {
            if (refObj.id == idRef) {
                refObj.gameObject.SendMessage(funcaoName);
            }
        }
    }

}
