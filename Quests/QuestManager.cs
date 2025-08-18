using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Quests.Quest;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        public Quest currentQuest;
        Quest lastActiveQuest;

        public List<Quest> allQuests = new();
        public List<Quest> activeQuests = new();
        public List<Quest> questsCompleted = new();

        public event Action<Quest> OnQuestAdded;
        public event Action<Quest> OnQuestActivated;
        public event Action<Quest> OnQuestDeactivated;
        public event Action<Quest> OnQuestUpdated;
        public event Action<Quest> OnQuestCompleted;

        QuestsSaveData saveData = new();

        /// <summary>
        /// dodaje zadanie do listy aktywnych zadañ oraz ustawia je jako obecnie aktywne
        /// </summary>
        /// <param name="quest">nowe zadanie</param>
        public void AddQuest(Quest quest)
        {
            if (!activeQuests.Contains(quest))
                activeQuests.Add(quest);

            SetActiveQuest(quest);
            OnQuestActivated?.Invoke(quest);
        }
        /// <summary>
        /// Odœwie¿a UI zadania
        /// </summary>
        /// <param name="quest">zadanie, któremu ma zostaæ odœwie¿one UI</param>
        public void NotifyQuestUpdated(Quest quest)
        {
            OnQuestUpdated?.Invoke(quest);
        }
        /// <summary>
        /// Ustawia zadanie na obecnie aktywne
        /// </summary>
        /// <param name="quest">zadabue do aktywancji</param>
        public void SetActiveQuest(Quest quest)
        {
            if (lastActiveQuest == quest)
            {
                ToggleQuestActivity(quest);
                return;
            }

            if (lastActiveQuest != null)
            {
                lastActiveQuest.isActive = false;
                OnQuestDeactivated?.Invoke(lastActiveQuest);
            }

            lastActiveQuest = quest;
            currentQuest = quest;
            quest.isActive = true;
            OnQuestActivated?.Invoke(quest);
        }
        /// <summary>
        /// prze³¹cz aktywne zadanie
        /// </summary>
        /// <param name="quest">zadanie do prze³¹czenia</param>
        void ToggleQuestActivity(Quest quest)
        {
            quest.isActive = !quest.isActive;
            if (quest.isActive)
                OnQuestActivated?.Invoke(quest);
            else
                OnQuestDeactivated?.Invoke(quest);
        }
        /// <summary>
        /// Ukoñcz zadanie
        /// </summary>
        /// <param name="quest">zadanie, które ma zostaæ zakoñczone</param>
        public void QuestCompleted(Quest quest)
        {
            if (!activeQuests.Contains(quest))
                questsCompleted.Add(quest);

            activeQuests.Remove(quest);
            quest.isActive = false;
            quest.state = QuestState.Completed;

            OnQuestCompleted?.Invoke(quest);

        }
        /// <summary>
        /// zapisz dane zadañ
        /// </summary>
        /// <returns></returns>
        public QuestsSaveData SaveQuests()
        {
            saveData.questsData.Clear();
            foreach (Quest quest in allQuests)
            {
                QuestData questData = new QuestData();

                questData.questID = quest.questID;
                questData.questState = quest.state;
                questData.questStage = quest.questStage;

                saveData.questsData.Add(questData);
            }
            return saveData;
        }
        /// <summary>
        /// wczytaj zadania
        /// </summary>
        /// <param name="data">wczytany plik</param>
        public void LoadQuests(GameData data)
        {
            QuestsSaveData questsData = data.questsData;
            if (questsData == null) return;

            foreach (QuestData questData in questsData.questsData)
            {
                Quest loaded = LoadedQuest(questData.questID);
                if (loaded == null) continue;

                loaded.state = questData.questState;

                // Dodaj do odpowiedniej listy
                if (loaded.state == QuestState.Active)
                {
                    if (!activeQuests.Contains(loaded))
                        activeQuests.Add(loaded);

                    // Ustaw jako currentQuest tylko raz (np. pierwszy aktywny quest)
                    if (currentQuest == null)
                        currentQuest = loaded;
                }
                else if (loaded.state == QuestState.Completed)
                {
                    if (!questsCompleted.Contains(loaded))
                        questsCompleted.Add(loaded);

                }

                // Ustaw etap questa
                loaded.SetStage(Mathf.Max(1, questData.questStage), fromLoad: true);

                // Poka¿ w UI
                OnQuestActivated?.Invoke(loaded);
            }

            // Jeœli mamy ustawiony currentQuest, odœwie¿ jego opis w UI
            if (currentQuest != null)
                OnQuestUpdated?.Invoke(currentQuest);

        }

        Quest LoadedQuest(int id) => allQuests.FirstOrDefault(q => q.questID == id);
    }
}