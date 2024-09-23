using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoComecaMissao : Acao {
    public string idMissao;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id da Missão" }; }

    public AcaoComecaMissao(string idMissao, string triggerName) {
        this.idMissao = idMissao;
    }

    public AcaoComecaMissao(AcaoParams parametros): base(parametros) {
        idMissao = parametros.id;
    }

    public override void Realizar() {
        QuestState questState = QuestManager.instance.GetQuestState(idMissao);
        
        if (questState == QuestState.CAN_START) {
            QuestManager.instance.StartQuest(idMissao);
        }
    }

}
