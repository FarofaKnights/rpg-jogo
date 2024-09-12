using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Objeto que são ativados/desativados de acordo com o estado de uma quest
public class QuestObject : MonoBehaviour {
    [Header("Quest Referenceable Object Info")]
    public QuestInfo questInfo;
    public string objectId;
}
