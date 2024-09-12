using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour {
    public static QuestManager instance;

    // Armazena todas as quests
    public Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    // Eventos chamados quando o estado de uma quest muda
    public Action<string> OnQuestStart, OnQuestAdvance, OnQuestFinish;
    public Action<Quest> OnQuestStateChanged;

    public Quest selectedQuest = null;

    // Em relação aos Steps de trigger (bem comum)
    class Quest_Trigger {
        public string questId;
        public string triggerName;
        public System.Action action;
    }

    List<Quest_Trigger> questTriggers = new List<Quest_Trigger>();

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        quests = LoadQuests();

        foreach (Quest quest in quests.Values) {
            if (OnQuestStateChanged != null) {
                OnQuestStateChanged(quest);
            }
        }

        foreach (Quest quest in quests.Values) {
            if (quest.info.requirementsInfo != null && quest.info.requirementsInfo.GetCondicao() != null) {
                Condicao condicao = quest.info.requirementsInfo.GetCondicao();
                condicao.Then(() => {
                    ChangeQuestState(quest.info.questId, QuestState.CAN_START);
                });
            } else {
                ChangeQuestState(quest.info.questId, QuestState.CAN_START);
            }
        }
    }

    public void StartQuest(string questId) {
        Quest quest = GetQuestById(questId);

        GameObject stepGO = quest.InstantiateStep(transform);
        stepGO.GetComponent<QuestStep>().Initialize(questId);

        ChangeQuestState(questId, QuestState.IN_PROGRESS);
    }

    public void AdvanceQuest(string questId) {
        Quest quest = GetQuestById(questId);
        
        if (quest.NextStep()) {
            GameObject stepGO = quest.InstantiateStep(transform);
            stepGO.GetComponent<QuestStep>().Initialize(questId);

            if (OnQuestAdvance != null) {
                OnQuestAdvance(questId);
            }
        } else {
            ChangeQuestState(questId, QuestState.FINISHED);
        }
    }

    public void FinishQuest(string questId) {
        Quest quest = GetQuestById(questId);
        ClaimReward(quest);
        ChangeQuestState(questId, QuestState.FINISHED);
    }

    void ClaimReward(Quest quest) {
        /*
        GameManager.instance.AddXP(quest.info.xpReward);
        GameManager.instance.AddCoins(quest.info.goldReward);
        */
        Debug.Log("ClaimReward não implementado");
    }

    void ChangeQuestState(string questId, QuestState state) {
        Quest quest = GetQuestById(questId);
        quest.state = state;

        if (OnQuestStateChanged != null) {
            OnQuestStateChanged(quest);
        }
    }

    bool CheckRequirements(Quest quest) {
        if (quest.info.requirementsInfo == null) {
            return true;
        }

        Condicao condicao = quest.info.requirementsInfo.GetCondicao();
        return condicao.Realizar();
    }

    public QuestState GetQuestState(string questId) {
        if (quests.ContainsKey(questId)) {
            return quests[questId].state;
        }

        return QuestState.WAITING_REQUIREMENTS;
    }

    // Carrega quests e salva em um dicionário
    Dictionary<string, Quest> LoadQuests() {
        Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

        // Carrega todos QuestInfo do [Resources/Quests]
        QuestInfo[] questInfos = Resources.LoadAll<QuestInfo>("Quests");

        foreach (QuestInfo questInfo in questInfos) {
            Quest quest = new Quest(questInfo);
            quests.Add(questInfo.questId, quest);
        }

        return quests;
    }

    // Pega uma quest pelo ID
    Quest GetQuestById(string questId) {
        if (quests.ContainsKey(questId)) {
            return quests[questId];
        }

        return null;
    }

    Quest_Trigger GetQuestTrigger(string questId, string triggerName) {
        foreach (Quest_Trigger trigger in questTriggers) {
            if (trigger.questId == questId && trigger.triggerName == triggerName) {
                return trigger;
            }
        }

        return null;
    }

    public void RegisterQuestTrigger(string questId, string triggerName, System.Action action) {
        Quest_Trigger trigger = GetQuestTrigger(questId, triggerName);
        if (trigger == null) {
            trigger = new Quest_Trigger();
            trigger.questId = questId;
            trigger.triggerName = triggerName;
            questTriggers.Add(trigger);
        }

        trigger.action += action;
    }

    public void TriggerQuest(string questId, string triggerName) {
        Quest_Trigger trigger = GetQuestTrigger(questId, triggerName);
        if (trigger != null) {
            trigger.action();
        }
    }

    public void ClearQuestTriggers(string questId, string triggerName) {
        for (int i = questTriggers.Count - 1; i >= 0; i--) {
            if (questTriggers[i].questId == questId && questTriggers[i].triggerName == triggerName) {
                questTriggers.RemoveAt(i);
            }
        }
    }
}
