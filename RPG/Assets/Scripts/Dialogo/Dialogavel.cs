using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[RequireComponent(typeof(Interagivel))]
public class Dialogavel : MonoBehaviour, Saveable {
    public string nome;

    public List<Dialogo> dialogos = new List<Dialogo>();
    List<Dialogo> disponiveis = new List<Dialogo>();
    List<Dialogo> aguardando = new List<Dialogo>();
    List<Dialogo> concluidos = new List<Dialogo>();

    Dialogo dialogoAtual;

    void Start() {
        // Separa entre dialogos prontos para serem exibidos e dialogos que estão aguardando condições para serem exibidos
        foreach (Dialogo dialogo in dialogos) {
            Condicao condicao = dialogo.condicao.GetCondicao();
            if (condicao == null) disponiveis.Add(dialogo);
            else if (condicao.CheckCondicao()) disponiveis.Add(dialogo);
            else aguardando.Add(dialogo);
        }

        // Adiciona eventos de conclusão para os dialogos que estão aguardando condições
        foreach (Dialogo dialogo in aguardando) {
            SetEventoConclusao(dialogo);
        }
    }

    void SetEventoConclusao(Dialogo dialogo) {
        dialogo.condicao.GetCondicao().Then(() => {
            disponiveis.Add(dialogo);
            aguardando.Remove(dialogo);
        });
    }

    void CheckAguardando() {
        for (int i = aguardando.Count - 1; i >= 0; i--) {
            Dialogo dialogo = aguardando[i];

            // Tenta realizar a condição (só será realizada se atender a condição interna)
            dialogo.condicao.GetCondicao().Realizar();
        }
    
    }

    Dialogo GetDialogo() {
        if (disponiveis.Count == 0) return null;

        // Pega o dialogo com maior prioridade
        int prioridade = int.MinValue;
        List<Dialogo> dialogosComPrioridade = new List<Dialogo>();
        foreach (Dialogo dialogo in disponiveis) {
            if (dialogo.prioridade > prioridade) {
                dialogosComPrioridade.Clear();
                dialogosComPrioridade.Add(dialogo);
                prioridade = dialogo.prioridade;
            } else if (dialogo.prioridade == prioridade) {
                dialogosComPrioridade.Add(dialogo);
            }
        }

        Dialogo dialogoEscolhido;
        if (dialogosComPrioridade.Count == 1) {
            dialogoEscolhido = dialogosComPrioridade[0];
        } else {
            dialogoEscolhido = dialogosComPrioridade[Random.Range(0, dialogosComPrioridade.Count)];
        }

        return dialogoEscolhido;
    }

    public void OnPlayerInteract() {
        CheckAguardando();
        dialogoAtual = GetDialogo();
        if (dialogoAtual == null) return;

        UIController.dialogo.StartDialogo(dialogoAtual.falas.ToArray(), () => {
            concluidos.Add(dialogoAtual);
            disponiveis.Remove(dialogoAtual);
            dialogoAtual = null;
        });
    }

    public JSONObject Save() {
        JSONObject json = new JSONObject();
        JSONObject concluidosJson = new JSONObject(JSONObject.Type.Array);
        JSONObject aguardandoJson = new JSONObject(JSONObject.Type.Array);

        foreach (Dialogo dialogo in concluidos) {
            concluidosJson.Add(dialogo.name);
        }

        foreach (Dialogo dialogo in aguardando) {
            aguardandoJson.Add(dialogo.name);
        }

        json.AddField("concluidos", concluidosJson);
        json.AddField("aguardando", aguardandoJson);

        return json;
    }

    public void Load(JSONObject json) {

        if (json.HasField("concluidos") && json.GetField("concluidos").list != null) {
            foreach (var val in json.GetField("concluidos").list) {
                string nomeDialogo = val.stringValue;
                Dialogo dialogo = dialogos.Find(d => d.name == nomeDialogo);

                if (dialogo != null) {
                    if (aguardando.Contains(dialogo)) {
                        aguardando.Remove(dialogo);
                    }

                    if (disponiveis.Contains(dialogo)) {
                        disponiveis.Remove(dialogo);
                    }

                    if (!concluidos.Contains(dialogo)) {
                        concluidos.Add(dialogo);
                    }
                }
            }
        }

        if (json.HasField("aguardando") && json.GetField("aguardando").list != null) {
            foreach (var val in json.GetField("aguardando").list) {
                string nomeDialogo = val.stringValue;
                Dialogo dialogo = dialogos.Find(d => d.name == nomeDialogo);
                if (dialogo != null) {
                    if (!aguardando.Contains(dialogo)) {
                        aguardando.Add(dialogo);
                        SetEventoConclusao(dialogo);
                    }

                    if (disponiveis.Contains(dialogo)) {
                        disponiveis.Remove(dialogo);
                    }
                }
            }
        }
    }
}
