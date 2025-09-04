using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quests 
{
    public abstract class Quest : MonoBehaviour
    {
        public enum QuestState 
        { 
            Inactive, 
            Active, 
            Completed 
        }
        public enum QuestType
        {
            Fabular,
            Side,
            Mission
        }
        public QuestDefinition questDefition;

        [Header("Identyfikatory")]
        public int questStage = 0;
        public QuestState state;

        [Header("Dialogi")]
        public string questProgress;

        public QuestType questType;

        [Header("DŸwiêki ogólne")]
        protected AudioManager audioManager;
        public QuestSounds questSounds;

        public bool isActive;

        [Header("Manager")]
        protected QuestManager questManager;

        QuestData questData = new();

        protected Dictionary<int, Action<bool>> enter = new();
        protected Dictionary<int, Action<bool>> exit = new();
        protected virtual void Awake()
        {
            audioManager = AudioManager.Instance;
            questManager = QuestManager.Instance;
        }
        protected void RegisterStage(int stage, Action<bool> onEnter, Action<bool> onExit)
        {
            if (onEnter != null) enter[stage] = onEnter;
            if (onExit != null) exit[stage] = onExit;
        }
        /// <summary>
        /// zacznij zadanie
        /// </summary>
        /// <param name="questID">ID zadania</param>
        public virtual void StartQuest(int questID)
        {
            state = QuestState.Active;
            SetStage(1);

            isActive = true;

            // Pobierz stan questa z QuestManagera i ustaw flagê
            var data = questManager?.GetQuestState(questID);

            if (data != null)
                data.isActive = true;

            if (questSounds.startMission != null)
                audioManager.PlayClip(questSounds.startMission);

            questManager?.StartQuest(questID);
        }
        public void StartQuestFromDefinition()
        {
            StartQuest(questDefition.questID);
        }
        /// <summary>
        /// przejdŸ do kolejnego etapu zadania
        /// </summary>
        /// <param name="fromLoad">czy z zapisu</param>
        public void AdvanceStage(bool fromLoad = false)
        {
            SetStage(questStage + 1, fromLoad);
        }
        /// <summary>
        /// ustaw etap zadania
        /// </summary>
        /// <param name="newStage">nowy etap zadania</param>
        /// <param name="fromLoad">czy z zapisu</param>
        public void SetStage(int newStage, bool fromLoad = false)
        {
            if (questStage == newStage) return;

            OnStageExit(questStage, fromLoad);

            int prevStage = questStage;
            questStage = newStage;

            questProgress = SafeStageText(questStage);

            // aktualizuj globalny stan
            var stateData = questManager?.GetQuestState(questDefition.questID);
            if (stateData != null)
                stateData.questStage = questStage;

            if (!fromLoad && questSounds.updateMission != null && questStage > 1 && prevStage > 0)
                audioManager?.PlayClip(questSounds.updateMission);

            OnStageEnter(questStage, fromLoad);

            // zawsze odœwie¿ UI
            questManager?.NotifyQuestUpdated(questDefition.questID);
        }
        /// <summary>
        /// ustaw nowy cel zadania w UI
        /// </summary>
        /// <param name="s">etap zadania</param>
        /// <returns></returns>
        protected virtual string SafeStageText(int s)
        {
            if (questDefition.stages != null && s - 1 >= 0 && s -1 < questDefition.stages.Length)
                return questDefition.stages[s - 1];
            return questProgress;
        }
        protected virtual void OnStageEnter(int s, bool fromLoad) { }
        protected virtual void OnStageExit(int s, bool fromLoad) { }

        /// <summary>
        /// zakoñcz zadanie
        /// </summary>
        public virtual void EndQuest(bool fromLoad)
        {
            state = QuestState.Completed;

            if (questSounds.endMission != null)
                audioManager?.PlayClip(questSounds.endMission);

            questManager?.CompleteQuest(questDefition.questID);
        }
        /// <summary>
        /// zmienia stan widocznoœci znacznika na mapie
        /// </summary>
        /// <param name="mark">znacznik</param>
        /// <param name="state">stan</param>
        protected void SetMapMark(GameObject mark, bool state) => mark.SetActive(state);

        /// <summary>
        /// uczytuje dane
        /// </summary>
        protected void HandleLoad()
        {
            questData = questManager.GetQuestState(questDefition.questID);

            if (questDefition == null)
            {
                Debug.LogError($"{name}: Brak przypisanej definicji questa");
                return;
            }

            if (questData != null)
            {
                questStage = questData.questStage;
                state = questData.questState;
                isActive = questData.isActive;

                // jeœli quest jest aktywny i ma ju¿ jakiœ etap > 0, odtwórz logikê wejœcia
                if (questStage > 0)
                {
                    OnStageEnter(questStage, fromLoad: true);
                    //uiController.UpdateMissionList(questData);
                }
            }

            if (isActive)
            {
                questManager.NotifyQuestUpdated(questDefition.questID);
                return;
            }
        }
    }
}