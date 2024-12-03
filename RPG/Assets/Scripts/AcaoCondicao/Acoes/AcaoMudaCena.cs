using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AcaoMudaCena : Acao {
    public string nome, point;
    public bool naoEhFase;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "id2", "boolValue"}; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Nome da cena", "Spawnpoint", "Não é fase" }; }

    public AcaoMudaCena(string nome, string point, bool naoEhFase) {
        this.nome = nome;
        this.point = point;
        this.naoEhFase = naoEhFase;
    }

    public AcaoMudaCena(AcaoParams parametros): base(parametros) {
        nome = parametros.id;
        point = parametros.id2;
        naoEhFase = parametros.boolValue;
    }

    public override void Realizar() {
        if (naoEhFase) {
            SceneManager.LoadScene(nome);
        } else {
            LevelInfo level = GameManager.instance.loading.GetLevelInfo(nome);
            GameManager.instance.GoToScene(level);
        }
    }

}
