using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AcaoTocaSom : Acao {
    public string pathAudio;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Path do Audio" }; }

    public AcaoTocaSom(string pathAudio) {
        this.pathAudio = pathAudio;
    }

    public AcaoTocaSom(AcaoParams parametros): base(parametros) {
        pathAudio = parametros.id;
    }

    public override void Realizar() {
        AudioManager.instance.PlayOnReservado(pathAudio);
    }

}
