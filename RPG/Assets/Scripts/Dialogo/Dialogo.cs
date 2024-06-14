using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fala {
    [TextArea(3, 10)]
    public string text;
    public bool autoNext = false; // Se a fala deve avançar automaticamente (bom para ações sem texto)
    public AcaoInfo acao;
}


[CreateAssetMenu(fileName = "Novo Dialogo", menuName = "RPG/Dialogo")]
public class Dialogo : ScriptableObject {
    public CondicaoInfo condicao;
    public int prioridade;
    public List<Fala> falas = new List<Fala>();
}
