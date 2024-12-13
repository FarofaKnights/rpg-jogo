using UnityEngine;

public class CutsceneExtras : MonoBehaviour {
    public QuestInfo questInfo;

    public void HandleClick() {
        GameManager.instance.PlayCutsceneFromExtras(questInfo);
    }
}
