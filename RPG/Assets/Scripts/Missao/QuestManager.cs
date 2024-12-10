using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Defective.JSON;

public class QuestManager : MonoBehaviour, Saveable {
    public static QuestManager instance;

    // Armazena todas as quests
    public Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    // Eventos chamados quando o estado de uma quest muda
    public Action<string> OnQuestStart, OnQuestAdvance, OnQuestFinish;
    public Action<Quest> OnQuestStateChanged;
    Action _OnQuestsLoaded;
    bool loadingFromSave = false;

    List<string> OnGoingQuests = new List<string>();

    public Action OnQuestsLoaded {
        get { return _OnQuestsLoaded; }
        set {
            if (quests.Count > 0) {
                value?.Invoke();
            } else {
                _OnQuestsLoaded = value;
            }
        }
    }

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
                    if (quest.state == QuestState.WAITING_REQUIREMENTS)
                        ChangeQuestState(quest.info.questId, QuestState.CAN_START);
                });
            } else if (quest.state == QuestState.WAITING_REQUIREMENTS){
                ChangeQuestState(quest.info.questId, QuestState.CAN_START);
            }
        }
        
        OnQuestsLoaded?.Invoke();

        CheckIfInLevel();
    }

    public void StartQuest(string questId) {
        if (OnGoingQuests.Contains(questId)) return;

        Quest quest = GetQuestById(questId);

        GameObject stepGO = quest.InstantiateStep(transform);
        stepGO.GetComponent<QuestStep>().Initialize(questId);

        OnGoingQuests.Add(questId);

        ChangeQuestState(questId, QuestState.IN_PROGRESS);
    }

    public void AdvanceQuest(string questId) {
        if (loadingFromSave) return; // Sim, isso é uma gambiarra
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
        UIController.HUD.UpdateMissaoText(quest.info, "");
    }

    public void SetFalhaQuest(string questId) {
        Quest quest = GetQuestById(questId);
        quest.SetFalha();
        ChangeQuestState(questId, QuestState.FINISHED);
        UIController.HUD.UpdateMissaoText(quest.info, "");
    }

    void ClaimReward(Quest quest) { /* Nunca existiu, nunca existirá */ }

    public void ChangeQuestState(string questId, QuestState state) {
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

    public bool IsQuestFinished(string questId) {
        return GetQuestState(questId) == QuestState.FINISHED;
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

    public JSONObject Save() {
        JSONObject obj = new JSONObject();

        JSONObject questsObj = new JSONObject();
        foreach (Quest quest in quests.Values) {
            if (quest.state == QuestState.WAITING_REQUIREMENTS) continue;
            questsObj.AddField(quest.info.questId, quest.Save());
        }

        obj.AddField("quests", questsObj);

        return obj;
    }

    public void Load(JSONObject obj) {
        loadingFromSave = true;
        JSONObject questsObj = obj.GetField("quests");
        if (questsObj == null || questsObj.list == null) return;

        for (int i = 0; i < questsObj.list.Count; i++) {
            string questId = questsObj.keys[i];
            JSONObject questObj = questsObj.list[i];

            if (quests.ContainsKey(questId)) {
                Quest quest = quests[questId];
                quest.Load(questObj);
            }
        }

        CheckIfInLevel();

        loadingFromSave = false;
    }

    public void RunQuestStepOnLoad(string questId) {
        Quest quest = GetQuestById(questId);
        if (quest.state == QuestState.IN_PROGRESS) {
            GameObject stepGO = quest.InstantiateStep(transform);
            stepGO.GetComponent<QuestStep>().Initialize(questId);
        }
    }

    public void CheckIfInLevel(){
        LevelInfo currentLevel = GameManager.instance.CurrentScene();

        foreach (Quest quest in quests.Values) {
            if (quest.state == QuestState.IN_PROGRESS || quest.state == QuestState.CAN_FINISH) {
                if (quest.info.level != currentLevel) {
                    SetFalhaQuest(quest.info.questId);
                }
            }
        }
    }
}
