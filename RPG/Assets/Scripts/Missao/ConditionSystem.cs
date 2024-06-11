using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSystem {
   Dictionary<string, System.Type> condicoes = new Dictionary<string, System.Type>();
   Dictionary<string, System.Type> acoes = new Dictionary<string, System.Type>();

   public void RegisterColisao<T>(string id) where T : Condicao {
       condicoes.Add(id, typeof(T));
   }

   public Condicao GetCondicao(string id, CondicaoParams parameters) {
        if (condicoes.ContainsKey(id)) {
            return System.Activator.CreateInstance(condicoes[id], parameters) as Condicao;
        }

        return null;
   }

    public void RegisterAcao<T>(string id) where T : Acao {
         acoes.Add(id, typeof(T));
    }

    public Acao GetAcao(string id, AcaoParams parameters) {
        if (acoes.ContainsKey(id)) {
            return System.Activator.CreateInstance(acoes[id], parameters) as Acao;
        }

        return null;
    }
}
