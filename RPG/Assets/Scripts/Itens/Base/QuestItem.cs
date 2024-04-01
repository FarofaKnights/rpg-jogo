using UnityEngine;

public class QuestItem : TipoAbstrato {
    public override bool IsInstanciavel { get { return false; } }
    public override bool IsUsavel { get { return false; } }
    public override void FazerAcao() {
        Debug.Log("QuestItem usado (ainda não faz nada sozinho)");
    }
}
