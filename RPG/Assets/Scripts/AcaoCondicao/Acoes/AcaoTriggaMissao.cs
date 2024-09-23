using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoTriggaMissao : Acao {
    public string idMissao, triggerName;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "stringValue" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id da Missão", "Nome do Trigger" }; }

    public AcaoTriggaMissao(string idMissao, string triggerName) {
        this.idMissao = idMissao;
        this.triggerName = triggerName;
    }

    public AcaoTriggaMissao(AcaoParams parametros): base(parametros) {
        idMissao = parametros.id;
        triggerName = parametros.stringValue;
    }

    public override void Realizar() {
        QuestState questState = QuestManager.instance.GetQuestState(idMissao);

        if (questState == QuestState.IN_PROGRESS || questState == QuestState.CAN_FINISH) {
            QuestManager.instance.TriggerQuest(idMissao, triggerName);
        }
    }

}
