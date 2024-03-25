using UnityEngine;

public class QuestItem : TipoAbstrato {
    public override void FazerAcao() {
        Debug.Log("QuestItem usado (ainda não faz nada sozinho)");
    }
}
