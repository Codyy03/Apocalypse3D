using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Quests.Quest;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;
        public int currentQuestID;

        readonly int noCurrentQuest = 0;
        int? lastActiveQuestID = null;

        [SerializeField] List<QuestDefinition> questDefinitions;

        Dictionary<int, QuestData> questStates = new();

        public event Action<QuestData> OnQuestActivated;
        public event Action<QuestData> OnQuestUpdated;
        public event Action<QuestData> OnQuestCompleted;
        public event Action<string> updateCustemQuestProgress;
        private void Awake()
        {
            Instance = this;

            foreach (var def in questDefinitions)
            {
                questStates[def.questID] = new QuestData
                {
                    questID = def.questID,
                    questStage = 0,
                    questState = QuestState.Inactive,
                };
            }
        }

        public void StartQuest(int questID)
        {
            if (!questStates.ContainsKey(questID)) return;

            currentQuestID = questID;

            questStates[questID].questState = QuestState.Active;
            questStates[questID].questStage = 1;

            OnQuestActivated?.Invoke(questStates[questID]);
        }
        /// <summary>
        /// Odœwie¿a UI zadania
        /// </summary>
        /// <param name="questID">zadanie, któremu ma zostaæ odœwie¿one UI</param>
        public void NotifyQuestUpdated(int questID)
        {
            if (questStates.TryGetValue(questID, out var state))
                OnQuestUpdated?.Invoke(state);
        }
        /// <summary>
        /// Ustawia zadanie na obecnie aktywne
        /// </summary>
        /// <param name="questID">zadabue do aktywancji</param>
        public void SetActiveQuest(int questID)
        {
            // Jeœli klikniêto ponownie ten sam quest prze³¹cz aktywnoœæ
            if (lastActiveQuestID.HasValue && lastActiveQuestID.Value == questID)
            {
                ToggleQuestActivity(questID);
                return;
            }

            // Dezaktywuj poprzedni, jeœli by³
            if (lastActiveQuestID.HasValue &&
                questStates.TryGetValue(lastActiveQuestID.Value, out var lastQuest))
            {
                lastQuest.isActive = false;
            }

            // Aktywuj nowy
            if (questStates.TryGetValue(questID, out var quest))
            {
                quest.isActive = true;
                currentQuestID = questID;
                lastActiveQuestID = questID;
                OnQuestActivated?.Invoke(quest);
            }
        }

        /// <summary>
        /// prze³¹cz aktywne zadanie
        /// </summary>
        /// <param name="questID">zadanie do prze³¹czenia</param>
        void ToggleQuestActivity(int questID)
        {
            if (!questStates.TryGetValue(questID, out var quest))
                return;

            quest.isActive = !quest.isActive;

            if (quest.isActive)
            {
                currentQuestID = quest.questID;
                OnQuestActivated?.Invoke(quest);
            }
            else
            {
                currentQuestID = noCurrentQuest;
                OnQuestUpdated?.Invoke(quest); // odœwie¿ tekst
            }
        }
        /// <summary>
        /// Ukoñcz zadanie
        /// </summary>
        /// <param name="questID">zadanie, które ma zostaæ zakoñczone</param>
        public void CompleteQuest(int questID)
        {
            if (!questStates.ContainsKey(questID)) return;

            currentQuestID = noCurrentQuest;
            questStates[questID].questState = Quest.QuestState.Completed;
            OnQuestCompleted?.Invoke(questStates[questID]);
        }

        public void SetCustomQuestProgress(string progress)
        {
            updateCustemQuestProgress?.Invoke(progress);
        }

        /// <summary>
        /// zapisz dane zadañ
        /// </summary>
        /// <returns></returns>
        public QuestsSaveData SaveQuests()
        {
            var save = new QuestsSaveData();
            foreach (var q in questStates.Values)
            {
                save.questsData.Add(new QuestData
                {
                    questID = q.questID,
                    questStage = q.questStage,
                    questState = q.questState,
                    isActive = q.isActive
                });
            }
            return save;
        }

        /// <summary>
        /// wczytaj zadania
        /// </summary>
        /// <param name="data">wczytany plik</param>
        public void LoadQuests(QuestsSaveData data)
        {
            if (data == null) return;

            foreach (var saved in data.questsData)
            {
                 questStates[saved.questID].questStage = saved.questStage;
                 questStates[saved.questID].questState = saved.questState;
                 questStates[saved.questID].isActive = saved.isActive;

                if (saved.isActive)
                {
                    currentQuestID = saved.questID;
                    OnQuestActivated.Invoke(saved);
                    SetActiveQuest(saved.questID);
                }
            }
        }

        public QuestData GetQuestState(int questID) =>
        questStates.ContainsKey(questID) ? questStates[questID] : null;

        public void SetQuestState(int questID, bool isActive)
        {
            var questData = questStates.FirstOrDefault(q => q.Key == questID);

            questData.Value.isActive = isActive;
        }

        public QuestDefinition GetQuestDefinition(int questID) =>
            questDefinitions.FirstOrDefault(q => q.questID == questID);

    }
}