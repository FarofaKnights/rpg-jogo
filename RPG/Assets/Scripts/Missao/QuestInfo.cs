using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStepType { ACAO, PAI }


[System.Serializable]
public class GameObjectParameter {
    public QuestStepType type = QuestStepType.ACAO;
    public GameObject gameObject;
    public string parameter;
    public string informativo;

    #if UNITY_EDITOR
    // Essa parte é referente a visualização do passo no editor
    public bool mostrandoConteudo = true;
    #endif
}

[System.Serializable]
public class GameObjectParameterParent : GameObjectParameter {
    public GameObjectParameter[] children;
}


[System.Serializable]
public class FalaCarregada {
    public string nome;
    public Fala[] falas;
}

public interface IQuestInformations {
    void HandleQuestInformations(QuestInfo questInfo, string parameter);
}

public interface IInformativoAtualizavel {
    void UpdateInformativo(System.Action informativoUpdate);
}

[CreateAssetMenu(fileName = "QuestInfo", menuName = "RPG/QuestInfo")]
public class QuestInfo : ScriptableObject {
    [Header("Quest Info")]
    public string questId;
    
    public string titulo;
    [TextArea(3, 10)] public string descricao;
    
    public FalaCarregada[] falasCarregadas;

    [Header("Quest Requirements")]
    public CondicaoInfo requirementsInfo;

    [Header("Quest Steps")]
    public GameObjectParameterParent[] steps;

    [Header("Quest Rewards")]
    public AcaoInfo[] rewardsInfo;

    void OnValidate() {
        // Define o ID da quest como o nome do objeto de forma automatica
        #if UNITY_EDITOR
        questId = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    public Fala[] GetFalas(string nome) {
        foreach (FalaCarregada falaCarregada in falasCarregadas) {
            if (falaCarregada.nome == nome) {
                return falaCarregada.falas;
            }
        }

        return null;
    }

    public FalaCarregada GetFalaCarregada(string nome) {
        foreach (FalaCarregada falaCarregada in falasCarregadas) {
            if (falaCarregada.nome == nome) {
                return falaCarregada;
            }
        }

        return null;
    }
}
