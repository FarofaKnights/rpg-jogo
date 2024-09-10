using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectParameter {
    public GameObject gameObject;
    public string parameter;
}

public interface IQuestInformations {
    void HandleQuestInformations(QuestInfo questInfo, string parameter);
}

[CreateAssetMenu(fileName = "QuestInfo", menuName = "RPG/QuestInfo")]
public class QuestInfo : ScriptableObject {
    [Header("Quest Info")]
    public string questId;
    
    public string titulo;
    [TextArea(3, 10)] public string descricao;
    
    public Fala[] introductionFalas;
    public Fala[] questFalas;
    public Fala[] finishingFalas;

    [Header("Quest Requirements")]
    public CondicaoInfo requirementsInfo;

    [Header("Quest Steps")]
    public GameObjectParameter[] steps;

    [Header("Quest Rewards")]
    public AcaoInfo[] rewardsInfo;

    void OnValidate() {
        // Define o ID da quest como o nome do objeto de forma automatica
        #if UNITY_EDITOR
        questId = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
