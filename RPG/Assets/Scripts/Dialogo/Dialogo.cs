using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Escolha {
    public string escolha;

    public enum TipoEscolha { FALA, DIALOGO }
    public TipoEscolha tipoEscolha;
    public FalaSemEscolha respostaFala;
    public Dialogo respostaDialogo;
}

[System.Serializable]
public class Escolhas {
    public enum TipoEscolhas { NULL, MENU, ESCOLHA }
    public TipoEscolhas tipoEscolhas;
    public int idEscolhaDeSaidaDoMenu = -1;

    public Escolha[] escolhas;
}

[System.Serializable]
public class Fala: FalaSemEscolha {
    public Escolhas escolhas;

    public Fala(string text) {
        this.text = text;
        acao = null;
    }

    public Fala() {
        text = "";
        acao = null;
    }

    public Fala(FalaSemEscolha falaSemEscolha) {
        text = falaSemEscolha.text;
        acao = falaSemEscolha.acao;
        autoNext = falaSemEscolha.autoNext;
    }
}

[System.Serializable]
public class FalaSemEscolha {
    [TextArea(3, 10)]
    public string text;
    public bool autoNext = false; // Se a fala deve avançar automaticamente (bom para ações sem texto)
    public AcaoInfo acao;

    public FalaSemEscolha(string text) {
        this.text = text;
        acao = null;
    }

    public FalaSemEscolha() {
        text = "";
        acao = null;
    }
}


[CreateAssetMenu(fileName = "Novo Dialogo", menuName = "RPG/Dialogo")]
public class Dialogo : ScriptableObject {
    public CondicaoInfo condicao;
    public int prioridade;
    public List<Fala> falas = new List<Fala>();
}
